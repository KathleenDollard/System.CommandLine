// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine
{
    public class ParseResult
    {
        private readonly List<ParseError> _errors = new List<ParseError>();
        private CommandDefinition _commandDefinition;

        internal ParseResult(
            IReadOnlyCollection<string> tokens,
            SymbolSet symbols,
            Parser parser,
            IReadOnlyCollection<string> unparsedTokens = null,
            IReadOnlyCollection<string> unmatchedTokens = null,
            IReadOnlyCollection<ParseError> errors = null,
            string rawInput = null)
        {
            Tokens = tokens ??
                     throw new ArgumentNullException(nameof(tokens));
            Symbols = symbols ??
                      throw new ArgumentNullException(nameof(symbols));
            Parser = parser ??
                     throw new ArgumentNullException(nameof(parser));

            UnparsedTokens = unparsedTokens;
            UnmatchedTokens = unmatchedTokens;
            RawInput = rawInput;

            if (errors != null)
            {
                _errors.AddRange(errors);
            }

            CheckForErrors();
        }

        internal Parser Parser { get; }

        public SymbolSet Symbols { get; }

        public IReadOnlyCollection<ParseError> Errors => _errors;

        public IReadOnlyCollection<string> Tokens { get; }

        public IReadOnlyCollection<string> UnmatchedTokens { get; }

        internal string RawInput { get; }

        public IReadOnlyCollection<string> UnparsedTokens { get; }

        public CommandDefinition CommandDefinition()
        {
            if (_commandDefinition == null)
            {
                return _commandDefinition = Symbols.CommandDefinition();
            }

            return _commandDefinition;
        }

        private void CheckForErrors()
        {
            foreach (var option in Symbols.FlattenBreadthFirst())
            {
                var error = option.Validate();

                if (error != null)
                {
                    _errors.Add(error);
                }
            }

            var commandDefinition = CommandDefinition();

            if (commandDefinition != null &&
                commandDefinition.SymbolDefinitions.OfType<CommandDefinition>().Any())
            {
                var symbol = this.Command();
                _errors.Insert(0, new ParseError(
                                  symbol.ValidationMessages.RequiredCommandWasNotProvided(),
                                  symbol));
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

        public Symbol this[string alias] => this.Command().Children[alias];
    }
}
