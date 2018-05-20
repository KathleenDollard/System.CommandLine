using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.CommandLine;
using System.Reflection;
using System.Linq;

namespace System.CommandLine.Pineapple
{
    public class CommandLineApp
    {
        private Func<ParseResult, int> _errorHandler = DefaultParseErrorHandler;
        private readonly List<SymbolDefinition> _symbols = new List<SymbolDefinition>();
        private Dictionary<CommandDefinitionBuilder, MethodInfo> Actions { get; } =
            new Dictionary<CommandDefinitionBuilder, MethodInfo>();

        public int Run(string args)
        {
            ParserConfiguration configuration = new ParserConfiguration(
                Actions.Keys.Select(item => item.BuildCommandDefinition()).Cast<SymbolDefinition>().ToList().AsReadOnly());

            var parser = new Parser(configuration);

            ParseResult result = parser.Parse(args);

            if (result.Errors.Count > 0)
            {
                return _errorHandler(result);
            }

            var command = result.Command();

            MethodInfo method = Actions.SingleOrDefault((item=>item.Key.Name == command.Name)).Value;
            ParameterInfo[] parameters = method.GetParameters();

            return (int) method.Invoke(null, parameters.Select(item => command[item.Name]).ToArray());
        }

        private static int DefaultParseErrorHandler(ParseResult result)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var parseError in result.Errors)
            {
                Console.Error.WriteLine(parseError.Message);
            }
            Console.ResetColor();


            Console.Error.WriteLine("Use --help to see available commands and options.");

            return 1;
        }

        public CommandOption AddOption(string name, string description)
            => AddOption(new[] { name }, description);

        public CommandOption AddOption(string[] aliases, string description)
        {
            var definition = new OptionDefinition(aliases, description);

            _symbols.Add(definition);

            return new CommandOption(definition);
        }

        public CommandOption AddOption<T>(string shortName, string longName, string description)
            => AddOption<T>(new[] { shortName, longName }, description);

        public CommandOption AddOption<T>(string name, string description)
            => AddOption<T>(new[] { name }, description);

        public CommandOption AddOption<T>(string[] aliases, string description)
        {
            var definition = new OptionDefinition(aliases, description,
            argumentDefinition: new ArgumentDefinitionBuilder().ParseArgumentsAs<T>());

            _symbols.Add(definition);

            return new CommandOption(definition);
        }

        public void OnParseError(Func<ParseResult, int> action)
        {
            _errorHandler = action ?? throw new ArgumentNullException(nameof(action));
        }

        public CommandLineDefinition AddCommand<T>(string name, Func<Option, T> action)
        {
            CommandDefinitionBuilder commandDefinitionBuilder = new CommandDefinitionBuilder(name);
            Actions.Add(commandDefinitionBuilder, action.Method);
            return new CommandLineDefinition(commandDefinitionBuilder);
        }
    }
}
