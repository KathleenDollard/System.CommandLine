using System.Collections.Generic;
using System.CommandLine.API;

namespace System.CommandLine.Result
{
    public class CommandLineResult : BaseResult
    {
        public Command Command { get; internal set; }
        public IEnumerable<string> UnmatchedTokens { get; internal set; }

    }
}
