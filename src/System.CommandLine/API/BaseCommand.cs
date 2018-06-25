using System;
using System.Collections.Generic;
using System.CommandLine.Common;
using System.Linq;
using System.Text;

namespace System.CommandLine.API
{

    public partial class BaseCommand : BaseSymbolPart<Command>, ICanParent
    {
        private protected BaseCommand(string name = default, string help = default)
            : base(name, help)
        { }

        private protected BaseCommand(string name = default, string help = default,
                CommandCollection commands = default,
                OptionCollection options = default)
            : base(name, help)
        {
            if (commands != null)
            {
                SubCommands = commands ;
            }
            if (options != null)
            {
                Options = options ?? OptionCollection.Create();
            }
        }

        private protected BaseCommand(string name = default, string help = default,
                IEnumerable<Command> commands = default,
                IEnumerable<Option> options = default)
             : base(name, help)
        {
             SubCommands.Add(commands?.ToArray());
            Options .Add(options?.ToArray());
        }


        // I'm wondering whether these are an inherent part of the definition or part of parsing. 
        public bool? TreatUnmatchedTokensAsErrors { get; }
        internal MethodBinder ExecutionHandler { get; }

        public CommandCollection SubCommands { get; } = new CommandCollection();
        public OptionCollection Options { get; } = new OptionCollection();

        internal BaseCommand AddCommands(params Command[] childCommands)
        {
            foreach (var childCommand in childCommands)
            {
                SubCommands.Add(childCommand);
            }
            return this;
        }

        internal BaseCommand AddOptions(params Option[] options)
        {
            foreach (var option in options)
            {
                Options.Add<Option>(option);
            }
            return this;
        }

        protected override void AcceptChildren(IVisitor<Command> visitor)
        {
            foreach (var command in SubCommands)
            { command.Accept(visitor); }
            if (visitor is IVisitorStart<Option> optionVisitor)
                foreach (var option in Options)
                { option.Accept(optionVisitor); }
        }

    }
}
