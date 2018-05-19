using System;
using System.Collections.Generic;
using System.CommandLine.Builder;

namespace System.CommandLine.Pineapple
{
    public class CommandLineApp
    {
        private Action<Command, ParseResult> _action;
        private Func<ParseResult, int> _errorHandler = DefaultParseErrorHandler;
        private readonly List<SymbolDefinition> _symbols = new List<SymbolDefinition>();
        private CommandOption _helpOption;

        public CommandLineApp()
        {
            _helpOption = AddOption("--help", "Show help output");
        }

        public int Run(Func<Option, Option, int> method, string[] args)
        {
            var parameters = method.Method.GetParameters();
            var parser = new Parser(new CommandDefinition(_symbols.ToArray()));

            var result = parser.Parse(args);

            if (result.Errors.Count > 0)
            {
                return _errorHandler(result);
            }

            if (result.HasOption("help"))
            {
                Console.WriteLine(result.CommandDefinition().HelpView());
                return 3;
            }

            _action?.Invoke(result.Command(), result);

            return 0;
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

        public CommandOption AddOption(string shortName, string longName, string description)
            => AddOption(new[] { shortName, longName }, description);

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

        public void OnExecute(Action action)
            => OnExecute((_, __) => action());

        public void OnExecute(Action<Command> action)
            => OnExecute((cmd, _) => action(cmd));

        public void OnExecute(Action<Command, ParseResult> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void OnParseError(Func<ParseResult, int> action)
        {
            _errorHandler = action ?? throw new ArgumentNullException(nameof(action));
        }
    }
}
