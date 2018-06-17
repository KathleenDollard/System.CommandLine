using System.Collections.Generic;

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

        public static CommandLine Create(string help = null)
         => new CommandLine(help);

        public static CommandLine Create(params BasePart[] subParts)
            => Create(default, subParts);

        public static new CommandLine Create(string help, params BasePart[] subParts)
            => Create(help)
               .AddSubParts(subParts);

        public new CommandLineResult Result => (CommandLineResult)((BasePart)this).Result;

    }


}
