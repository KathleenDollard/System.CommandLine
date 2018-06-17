using System.Collections.Generic;

namespace System.CommandLine
{
    public class CommandLineResult : BaseResult
    {
        public Command Command { get; internal set; }
        public IEnumerable<string> UnmatchedTokens { get; internal set; }
    }
}
