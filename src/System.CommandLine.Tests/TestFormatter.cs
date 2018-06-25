using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.Formatters
{
    public class TestFormatter
    {
        private int _i = 0;

        public string Parse(CommandLine commandLine)
            => Parse((BaseCommand)commandLine);

        public string Parse(BaseCommand command)
        {
            var ret = $"\r\n{Spaces(_i)} [ {command.Name}";
            _i += 5;
            ret += ParseArgument(command) + Parse(command.Options) + Parse(command.SubCommands) + " ]";
            _i -= 5;
            return ret;
        }

        private string Spaces(int count)
            => new string(' ', count);

        private object ParseArgument(BaseCommand command)
            => command is IHasArgument commandWithArg
                ? Parse(commandWithArg.Argument)
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

        public string Parse(Argument argument)
            => argument == null
               ? ""
               : $"\r\n{Spaces(_i)}argument: { argument.Name } ";

    }
}
