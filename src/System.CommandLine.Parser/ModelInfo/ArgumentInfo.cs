using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Parser
{
    internal class ArgumentInfo :BaseInfo
    {
        public ArgumentInfo(ArgumentList argument)
           => Argument = argument;

        public ArgumentList Argument { get; }
    }
}
