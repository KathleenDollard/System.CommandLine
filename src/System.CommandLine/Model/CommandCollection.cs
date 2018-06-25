using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine
{
    public class CommandCollection : IEnumerable<Command>
    {
        private readonly List<Command> _commands = new List<Command>();

        internal CommandCollection()
        { }

        public CommandCollection Add(params Command[] commands)
        {
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    Add<Command>(command);
                }
            }
            return this;
        }

        // Not sure we want the following
        public TNewCommand Add<TNewCommand>(TNewCommand newCommand)
            where TNewCommand : Command
        {
            _commands.Add(newCommand);
            return newCommand;
        }

        public IEnumerator<Command> GetEnumerator()
            => _commands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<Command>)_commands).GetEnumerator();

        public Command this[string idOrName]
            => _commands.FirstOrDefault(x => x.Id == idOrName)
               ?? _commands.FirstOrDefault(x => x.Name == idOrName);

    }


}
