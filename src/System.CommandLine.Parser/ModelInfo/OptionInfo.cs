using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Parser
{
    internal class OptionInfo : BaseInfo 
    {
        public OptionInfo(Option option)
          => Option = option;

        public Option Option { get; }

        internal IEnumerable<string> ConstructedAliases { get; set; }

    }
}
