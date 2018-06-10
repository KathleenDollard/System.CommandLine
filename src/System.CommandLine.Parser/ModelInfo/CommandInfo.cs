using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Parser
{
    internal class CommandInfo : BaseInfo
    {
        public CommandInfo(Command command)
            => Command = command;

        public Command Command { get; }

        public IEnumerable<Command> SubCommandInfo { get; } = new List<Command>();
        public IEnumerable<Option> Options { get; } = new List<Option>();

    }
}
