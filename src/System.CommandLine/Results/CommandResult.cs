using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine
{
    public class CommandResult : BaseResult
    {
        public CommandResult(Command command)
        {
            Command = command;
        }

        internal Command Command { get; }
        public string SpecifiedToken { get; internal set; }

        public string Name => Command.Name;

        public IReadOnlyCollection<OptionResult> OptionResults
                => Command.Options
                        .Where(x => x.Result.IsUsed)
                        .Select(x => x.Result)
                        .ToList().AsReadOnly();

        public CommandResult SubCommandResult
            => Command.SubCommands
                .Where(x => x.Result.IsUsed)
                .Select(x => x.Result)
                .FirstOrDefault();

        public IEnumerable<object> GetArgumentValues()
        {

            if (Command is IHasArgument withArgument)
            {
                return withArgument.Result.Values;
            }
            return null;
        }

        public IEnumerable<T> GetArgumentValues<T>()
        {

            if (Command is IHasArgument withArgument)
            {
                if(withArgument.Result is ArgumentResult<T> resultOfT)
                {
                    return resultOfT.Values;
                }
                return withArgument.Result.Values.OfType<T>();
            }
            return null;
        }
    }
}
