using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Common;
using System.Linq;
using System.Text;

namespace System.CommandLine
{

    public class Command : BaseCommand, ICanParent
    {
        private protected Command(string name = default, string help = default)
            : base(name, help)
        { }

        private protected Command(string name = default, string help = default,
                IEnumerable<Command> commands = default,
                IEnumerable<Option> options = default)
            : base(name, help, commands, options)
        { }

        private protected Command(string name = default, string help = default,
                 CommandCollection commands = default,
                 OptionCollection options = default)
            : base(name, help, commands, options)
        { }


        public static Command Create(string name)
            => new Command(name, default);

        public static Command<T> Create<T>(string name, string help = default,
              Argument<T> argument = default,
                     CommandCollection commands = default,
                     OptionCollection options = default)
              => new Command<T>(name, help, argument, commands, options);

        public static Command Create(string name, string help = default,
                     CommandCollection commands = default,
                     OptionCollection options = default)
              => new Command(name, help, commands, options);

        public static Command<T> Create<T>(string name, string help = default,
             Argument<T> argument = default)
              => new Command<T>(name, help, argument);

        public static Command<T> Create<T>(string name, string help = default,
             Argument<T> argument = default,
             IEnumerable<Command> commands = default,
             IEnumerable<Option> options = default)
              => new Command<T>(name, help, argument, commands, options);

        public static Command Create(string name, string help = default,
             IEnumerable<Command> commands = default,
             IEnumerable<Option> options = default)
              => new Command(name, help, commands, options);

        public static Command Create(string name, string help = default)
            => new Command(name, help);

    }

    public class Command<T> : Command, IHasArgument
    {
        internal Command(string name = default,
                     string help = default,
                     Argument<T> argument = default)
                : this(name, help, default, default)
        { }

        internal Command(string name = default,
                   string help = default,
                   Argument<T> argument = default,
                   CommandCollection commands = default,
                   OptionCollection options = default)
               : base(name, help, commands, options)
        {
            Argument = argument ?? Argument<T>.MakeArgument();
        }

        internal Command(string name = default,
                 string help = default,
                 Argument<T> argument = default,
                 IEnumerable<Command> commands = default,
                 IEnumerable<Option> options = default)
           : base(name, help, commands, options)
        {
            Argument = argument ?? Argument<T>.MakeArgument();
        }


        public Argument<T> Argument { get; internal set; }

        Argument IHasArgument.Argument
            => Argument;

        protected override void AcceptChildren(IVisitor<Command> visitor)
        {
            if (visitor is IVisitorStart<Argument> argumentVisitor)
            {
                Argument.Accept(argumentVisitor);
            }
            base.AcceptChildren(visitor);
        }


    }


}
