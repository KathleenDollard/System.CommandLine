using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine
{
    public class CommandLine : Command
    {
        internal CommandLine(string help = null) : base(default, help)
        { }

        internal void ApplyResults(Command command, IEnumerable<string> unmatchedTokens)
        {
            var result = new CommandLineResult();
            result.Command = command;
            result.UnmatchedTokens = unmatchedTokens;
            SetResult(result);
        }

        private static CommandLine Create(string help = null, params BasePart[] subParts)
            => Create(help)
               .AddSubParts(subParts);

        public static CommandLine Create(string help = null,
            IEnumerable<Command> commands = null,
            IEnumerable<Option> options = null)
            => new CommandLine(help)
                .WithCommands(commands.ToArray())
                .WithOptions(options.ToArray());


        public static CommandLine<T> Create<T>(string help = null,
            ArgumentList<T> argument = null,
            IEnumerable<Command> commands = null,
            IEnumerable<Option> options = null)
            => (CommandLine<T>) new CommandLine<T>(help)
               .WithArgumentList(argument )
               .WithCommands(commands.ToArray())
               .WithOptions(options.ToArray());

        public new CommandLineResult Result => (CommandLineResult)((BasePart)this).Result;

    }

    public class CommandLine<T> : Command<T>
    {
        internal CommandLine(string help = null) : base(default, help)
        { }

        internal void ApplyResults(Command command, IEnumerable<string> unmatchedTokens)
        {
            var result = new CommandLineResult();
            result.Command = command;
            result.UnmatchedTokens = unmatchedTokens;
            SetResult(result);
        }

        public new CommandLineResult Result => (CommandLineResult)((BasePart)this).Result;

    }

}
