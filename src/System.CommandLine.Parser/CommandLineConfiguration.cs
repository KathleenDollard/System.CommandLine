using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace System.CommandLine.Parser
{
    // KAD: The name of this file seems odd
    public class CommandLineConfiguration
    {
        private IReadOnlyCollection<InvocationMiddleware> _middlewarePipeline;

        public CommandLineConfiguration(
            Command command,
            IReadOnlyCollection<char> argumentDelimiters = null,
            IReadOnlyCollection<string> prefixes = null,
            bool allowUnbundling = true,
            ValidationMessages validationMessages = null,
            ResponseFileHandling responseFileHandling = default,
            IReadOnlyCollection<InvocationMiddleware> middlewarePipeline = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            // Why do you have to have an option, argument or subcommand? Removed check here

            // Why is ArgumentDelimiters state?
            ArgumentDelimiters = argumentDelimiters ?? new[] { ':', '=' };
            CheckForDelimiterUsage(command, ArgumentDelimiters);

            var symbolDefinitions = command.Options.OfType<BaseSymbolPart>().Union(command.Commands);

            // TODO: Not yet doing the non root scenario
            if (symbolDefinitions.Count() == 1 &&
                symbolDefinitions.Single() is Command rootCommandDefinition)
            {
                RootCommand = rootCommandDefinition;
            }
            //else
            //{
            //    RootCommand = new Command(
            //        ParserUtils.ExeName,
            //        "",
            //        symbolDefinitions);
            //}

            SymbolDefinitions.Add(RootCommand);

            AllowUnbundling = allowUnbundling;
            ValidationMessages = validationMessages ?? ValidationMessages.Instance;
            ResponseFileHandling = responseFileHandling;
            _middlewarePipeline = middlewarePipeline;
            Prefixes = prefixes;
            FixOptionPrefixes(command, prefixes);
        }

        private static void FixOptionPrefixes(Command command, IReadOnlyCollection<string> prefixes)
        {
            if (prefixes?.Count > 0)
            {
                foreach (Option option in command.Options)
                {
                    var workingAliases = option.Aliases.Select(x => StripPrefix(x));
                    var constructedAliases = new List<string>();
                    foreach (string alias in workingAliases)
                    {
                        foreach (var prefix in prefixes)
                        {
                            constructedAliases.Add(prefix + alias);
                        }
                    }
                    option.ConstructedAliases = constructedAliases;
                }
                string StripPrefix(string alias)
                {
                    foreach (string prefix in prefixes)
                    {
                        if (alias.StartsWith(prefix))
                        { alias.Replace(prefix, ""); }
                    }
                    return alias;
                }
            }
        }

        private static void CheckForDelimiterUsage(Command command, IEnumerable<char> argumentDelimiters)
        {
            foreach (var delimiter in argumentDelimiters)
            {
                command.Options.Select(o => o.Aliases
                                .Select(x => CheckDelimiter(x, delimiter)));
                command.Options.Select(c => CheckDelimiter(c.Name, delimiter));
                command.Commands.Select(c => CheckDelimiter(c.Name, delimiter));
                if (command is IHasArgument withArgument)
                {
                    CheckDelimiter(withArgument.Argument.Name, delimiter);
                }
            }

            int CheckDelimiter(string x, char delimiter) => x.Contains(delimiter)
                                                     ? throw new ArgumentException($"Symbol cannot contain delimiter: {delimiter}")
                                                     : 0;
        }

        public IReadOnlyCollection<string> Prefixes { get; }

        public  List<BaseSymbolPart> SymbolDefinitions { get; } = new List<BaseSymbolPart>();

        public IReadOnlyCollection<char> ArgumentDelimiters { get; }

        public bool AllowUnbundling { get; }

        public ValidationMessages ValidationMessages { get; }

        internal IReadOnlyCollection<InvocationMiddleware> InvocationList =>
            _middlewarePipeline ??
            (_middlewarePipeline = new List<InvocationMiddleware>());

        internal Command RootCommand { get; }

        internal ResponseFileHandling ResponseFileHandling { get; }
    }
}
