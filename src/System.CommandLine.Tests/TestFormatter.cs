using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Formatters
{
    public class TestFormatter
    {
        private int _i = 0;

        public string Parse(CommandLine commandLine)
            => Parse((Command)commandLine);

        public string Parse(Command command)
        {
            var ret = $"\r\n{Spaces(_i)} [ {command.Name} {ParseArgument(command)} ";
            _i += 5;
            ret += Parse(command.Options) + Parse(command.Commands) + " ]";
            _i -= 5;
            return ret;
        }

        private string Spaces(int count)
            => new string(' ', count);

        private object ParseArgument(Command command)
            => command is IHasArgument commandWithArg
                ? Parse(commandWithArg.BaseArgument)
                : null;

        public string Parse(IEnumerable<Option> options)
            => options.Any()
               ? $"\r\n{Spaces(_i)}options: { string.Join(", ", options.Select(o => Parse(o)))}"
               : null;

        public string Parse(Option option)
            => $" {option.Name} ";

        public string Parse(IEnumerable<Command> commands)
            => commands.Any()
               ? $"\r\n{Spaces(_i)}subCommands: { string.Join(", ", commands.Select(c => Parse(c)))}"
               : null;

        public string Parse(BaseArgument argument)
            => $"args: { argument.Name } ";

    }
}
