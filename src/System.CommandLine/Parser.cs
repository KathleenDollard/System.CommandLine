// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.CommandLine
{
    public class Parser
    {
        private readonly ParserConfiguration _configuration;

        public Parser(params SymbolDefinition[] symbolDefinitions) : this(new ParserConfiguration(symbolDefinitions))
        {
        }

        public Parser(ParserConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public SymbolDefinitionSet SymbolDefinitions => _configuration.SymbolDefinitions;

        public virtual ParseResult Parse(IReadOnlyCollection<string> rawTokens, string rawInput = null)
        {
            var lexResult = NormalizeRootCommand(rawTokens).Lex(_configuration);
            var unparsedTokens = new Queue<Token>(lexResult.Tokens);
            var rootSymbols = new SymbolSet();
            var allSymbols = new List<Symbol>();
            var errors = new List<ParseError>(lexResult.Errors);
            var unmatchedTokens = new List<string>();

            var optionQueue = GatherOptions(SymbolDefinitions).ToList();

            while (unparsedTokens.Any())
            {
                var token = unparsedTokens.Dequeue();

                if (token.Type == TokenType.EndOfArguments)
                {
                    // stop parsing further tokens
                    break;
                }

                if (token.Type != TokenType.Argument)
                {
                    var definedOption =
                        SymbolDefinitions.SingleOrDefault(o => o.HasAlias(token.Value));

                    if (definedOption != null)
                    {
                        var parsedOption = allSymbols
                            .LastOrDefault(o => o.HasAlias(token.Value));

                        if (parsedOption == null)
                        {
                            parsedOption = Symbol.Create(definedOption, token.Value, validationMessages: _configuration.ValidationMessages);

                            rootSymbols.Add(parsedOption);
                        }

                        allSymbols.Add(parsedOption);

                        continue;
                    }
                }

                var added = false;

                foreach (var topLevelSymbol in Enumerable.Reverse(allSymbols))
                {
                    Symbol option = topLevelSymbol.TryTakeToken(token);

                    if (option != null)
                    {
                        allSymbols.Add(option);
                        added = true;
                        if (token.Type is TokenType.Option)
                        {
                            var existing = optionQueue.FirstOrDefault(name => name == option.Name);
                            if (existing != null)
                            {
                                // we've used this option - don't use it again
                                optionQueue.Remove(existing);
                            }
                        }
                        break;
                    }

                    if (token.Type == TokenType.Argument &&
                        topLevelSymbol.SymbolDefinition is CommandDefinition)
                    {
                        var optionName = optionQueue.FirstOrDefault();
                        if (optionName != null)
                        {
                            optionQueue.RemoveAt(0);
                            var newToken = new Token("-" + optionName, TokenType.Option);
                            option = topLevelSymbol.TryTakeToken(newToken);
                            if (option != null)
                            {
                                allSymbols.Add(option);
                                option = topLevelSymbol.TryTakeToken(token);
                                if (option != null)
                                {
                                    allSymbols.Add(option);
                                    added = true;
                                }
                            }
                        }
                        break;
                    }
                }

                if (!added)
                {
                    unmatchedTokens.Add(token.Value);
                }
            }

            if (rootSymbols.CommandDefinition()?.TreatUnmatchedTokensAsErrors == true)
            {
                errors.AddRange(
                    unmatchedTokens.Select(token => new ParseError(_configuration.ValidationMessages.UnrecognizedCommandOrArgument(token))));
            }

            return new ParseResult(
                rawTokens,
                rootSymbols,
                _configuration,
                unparsedTokens.Select(t => t.Value).ToArray(),
                unmatchedTokens,
                errors,
                rawInput);
        }

        private IEnumerable<string> GatherOptions(SymbolDefinitionSet symbolDefinitions)
        {
            var optionList = new List<string>();
            foreach (var symDef in symbolDefinitions) //.Where( s => s is OptionDefinition))
            {
                if (symDef is OptionDefinition)
                {
                    var validator = symDef.ArgumentDefinition.SymbolValidators.FirstOrDefault();
                    if (validator != null)
                    {
                        if (validator.Method.Name.Contains("ExactlyOne"))
                        {
                            optionList.Add(symDef.Name);
                        }
                    }
                }

                optionList.AddRange(GatherOptions(symDef.SymbolDefinitions));
            }

            return optionList;
        }

        internal IReadOnlyCollection<string> NormalizeRootCommand(IReadOnlyCollection<string> args)
        {
            var firstArg = args.FirstOrDefault();

            if (SymbolDefinitions.Count != 1)
            {
                return args;
            }

            var commandName = SymbolDefinitions
                              .OfType<CommandDefinition>()
                              .SingleOrDefault()
                              ?.Name;

            if (commandName == null ||
                string.Equals(firstArg, commandName, StringComparison.OrdinalIgnoreCase))
            {
                return args;
            }

            if (firstArg != null &&
                firstArg.Contains(Path.DirectorySeparatorChar) &&
                (firstArg.EndsWith(commandName, StringComparison.OrdinalIgnoreCase) ||
                 firstArg.EndsWith($"{commandName}.exe", StringComparison.OrdinalIgnoreCase)))
            {
                args = new[] { commandName }.Concat(args.Skip(1)).ToArray();
            }
            else
            {
                args = new[] { commandName }.Concat(args).ToArray();
            }

            return args;
        }
    }
}
