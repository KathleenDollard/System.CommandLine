// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Parser
{
    internal class ParseResultMaker
    {
        private readonly List<ParseError> _errors = new List<ParseError>();

        internal ParseResultMaker(
            CommandInfo rootCommand,
            CommandInfo commandToExecute,
            IReadOnlyCollection<string> tokens,
            IReadOnlyCollection<string> unparsedTokens,
            IReadOnlyCollection<string> unmatchedTokens,
            IReadOnlyCollection<ParseError> errors,
            string rawInput)
        {
            RootCommand = rootCommand;
            Command = commandToExecute;
            Tokens = tokens;
            UnparsedTokens = unparsedTokens;
            UnmatchedTokens = unmatchedTokens;
            RawInput = rawInput;

            if (errors != null)
            {
                _errors.AddRange(errors);
            }

            AddImplicitOptionsAndCheckForErrors();
        }

        public CommandInfo Command { get; }

        public CommandInfo RootCommand { get; }

        public IReadOnlyCollection<ParseError> Errors => _errors;

        public IReadOnlyCollection<string> Tokens { get; }

        public IReadOnlyCollection<string> UnmatchedTokens { get; }

        internal string RawInput { get; }

        public IReadOnlyCollection<string> UnparsedTokens { get; }

        private void AddImplicitOptionsAndCheckForErrors()
        {

            Action<Option> action = SetImplicitOptions;

            void SetImplicitOptions(Option option)
            {
                if (option.IsDefault)
                option.Result.IsUsed = true;
                if (option is )
            }

            void
            var optionErrors = RootCommand
            foreach (var option in RootCommand.Options)
            {

            }

            // This code looks weird as it doesn't recurse - it seems to look at the options and sets defaults
            foreach (var symbol in RootCommand.AllSymbols().ToArray())
            {
                if (symbol is Command command)
                {
                    foreach (var definition in command.Definition.SymbolDefinitions)
                    {
                        if (definition.ArgumentDefinition.HasDefaultValue &&
                            command.Children[definition.Name] == null)
                        {
                            switch (definition)
                            {
                                case OptionDefinition optionDefinition:
                                    command.AddImplicitOption(optionDefinition);
                                    break;
                            }
                        }
                    }

                    if (command.Definition.ArgumentDefinition.HasDefaultValue &&
                        command.Arguments.Count == 0)
                    {
                        switch (command.Definition.ArgumentDefinition.GetDefaultValue())
                        {
                            case string arg:
                                command.TryTakeToken(new Token(arg, TokenType.Argument));
                                break;
                        }
                    }
                }

                var error = symbol.Validate();

                if (error != null)
                {
                    _errors.Add(error);
                }
            }

            if (Command.Definition?.SymbolDefinitions.OfType<CommandDefinition>().Any() == true)
            {
                _errors.Insert(0,
                               new ParseError(
                                   Command.ValidationMessages.RequiredCommandWasNotProvided(),
                                   Command));
            }
        }

        public object ValueForOption(
            string alias) =>
            ValueForOption<object>(alias);

        public T ValueForOption<T>(
            string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(alias));
            }

            return this[alias].GetValueOrDefault<T>();
        }

        public BaseSymbolPart this[string alias] => Command.Children[alias];

        public override string ToString() => $"{nameof(ParseResultMaker)}: {this.Diagram()}";
    }
}
