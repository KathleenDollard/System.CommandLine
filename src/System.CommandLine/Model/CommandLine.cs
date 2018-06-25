using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine
{
    public class CommandLine : BaseCommand
    {
        private protected CommandLine(string name = default, string help = default,
                IEnumerable<Command> commands = default,
                IEnumerable<Option> options = default)
            : base(name, help, commands, options)
        { }

        private protected CommandLine(string name = default, string help = default,
                 CommandCollection commands = default,
                 OptionCollection options = default)
            : base(name, help, commands, options)
        { }

        public static CommandLine<T> Create<T>(string name, string help = default,
                 Argument<T> argument = default,
                 CommandCollection commands = default,
                 OptionCollection options = default)
              => new CommandLine<T>(name, help, argument, commands, options);

        public static CommandLine<T> Create<T>(string name, string help = default,
                 Argument<T> argument = default)
              => new CommandLine<T>(name, help, argument, default, default);

        public static CommandLine Create(string name, string help = default,
                CommandCollection commands = default,
                OptionCollection options = default)
              => new CommandLine(name, help, commands, options);

        public static CommandLine Create(string name, string help = default,
                 OptionCollection options = default)
              => new CommandLine(name, help, default, options);

        public static CommandLine Create(string name, string help = default,
                 CommandCollection commands = default)
              => new CommandLine(name, help, commands, default);

        public static CommandLine Create(string name, 
                OptionCollection options = default)
             => new CommandLine(name, default, default, options);

        public static CommandLine Create(string name, 
                 CommandCollection commands = default)
              => new CommandLine(name, default, commands, default);

        public static CommandLine<T> Create<T>(string name, string help = default,
                 Argument<T> argument = default,
                 IEnumerable<Command> commands = default,
                 IEnumerable<Option> options = default)
              => new CommandLine<T>(name, help, argument, commands, options);

        public static CommandLine Create(string name, string help = default,
                 IEnumerable<Command> commands = default,
                 IEnumerable<Option> options = default)
              => new CommandLine(name, help,  commands, options);

        public static CommandLine Create(string name, string help = default,
                 IEnumerable<Option> options = default)
              => new CommandLine(name, help, default, options);

        public static CommandLine Create(string name, string help = default,
                 IEnumerable<Command> commands = default)
              => new CommandLine(name, help, commands, default);

        public static CommandLine Create(string name, 
                  IEnumerable<Option> options = default)
               => new CommandLine(name, default, default, options);

        public static CommandLine Create(string name, 
                 IEnumerable<Command> commands = default)
              => new CommandLine(name, default, commands, default);

        internal void ApplyResults(Command command, IEnumerable<string> unmatchedTokens)
        {
            var result = new CommandLineResult();
            result.Command = command;
            result.UnmatchedTokens = unmatchedTokens;
            SetResult(result);
        }

        public new CommandLineResult Result => (CommandLineResult)((BasePart)this).Result;

    }

    public class CommandLine<T> : CommandLine, IHasArgument
    {
        internal CommandLine(string name = default,
                    string help = default,
                    Argument<T> argument = default,
                    CommandCollection commands = default,
                    OptionCollection options = default)
                : base(name, help, commands, options)
        {
            Argument = argument ?? Argument<T>.MakeArgument();
        }

        internal CommandLine(string name = default,
                 string help = default,
                 Argument<T> argument = default,
                 IEnumerable<Command> commands = default,
                 IEnumerable<Option> options = default)
           : base(name, help, commands, options)
        {
            Argument = argument ?? Argument<T>.MakeArgument();
        }

        public Argument<T> Argument { get; internal set; }

        Argument IHasArgument.Argument
            => Argument;

        protected override void AcceptChildren(IVisitor<Command> visitor)
        {
            if (visitor is IVisitorStart<Argument> argumentVisitor)
            {
                Argument.Accept(argumentVisitor);
            }
            base.AcceptChildren(visitor);
        }
    }

}
