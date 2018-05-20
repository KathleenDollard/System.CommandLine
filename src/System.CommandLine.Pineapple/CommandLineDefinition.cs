using System;
using System.Collections.Generic;
using System.CommandLine.Builder;
using System.Text;

namespace System.CommandLine.Pineapple
{
    public class CommandLineDefinition
    {
        CommandDefinitionBuilder CommandDefinitionBuilder { get; }

        public CommandLineDefinition(CommandDefinitionBuilder commandDefinitionBuilder)
        {
            CommandDefinitionBuilder = commandDefinitionBuilder;
        }

        public void AddOption(string shortName, string LongName, string Desciption)
        {
            CommandDefinitionBuilder.AddOption(new[] { shortName, LongName }, Desciption, arguments: a => a.WithHelp(name: "inner-args").ExactlyOne());
        }
    }
}
