using System.Collections.Generic;

namespace System.CommandLine.Parser
{
    internal class LexResult
    {
        public IEnumerable<Token> Tokens { get; set; }
        public IEnumerable<ParseError> Errors { get; set; }
    }
}
