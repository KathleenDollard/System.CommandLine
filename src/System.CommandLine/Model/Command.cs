using System;
using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Common;
using System.Linq;
using System.Text;

namespace System.CommandLine
{
    // KAD: Command questions:
    //    Why was aliases on Symbol. Do commands have aliases?
    //    How does a Token differ from a name? I kept it, but conflated values.

    // Exactly like Command, except different constructor to aid in learning

    public static class CommandExtensions
    {
        internal static TCmd AddArgument<TCmd, T>(this TCmd command, ArgumentList<T> argument)
            where TCmd : Command<T>
        {
            argument.Parent = command;
            command.Argument = argument;
            return command;
        }

        internal static TCmd AddSubParts<TCmd>(this TCmd command, BasePart[] subParts)
            where TCmd : Command
        {
            foreach (var part in subParts)
            {
                command.AddPart(part);

            }
            return command;
        }

        internal static void AddPart<TCmd>(this TCmd command, BasePart part)
            where TCmd : Command
        {
            part.Parent = command;
            switch (part)
            {
                case Option option:
                    command.Options.AddOption(option);
                    break;
                case Command subCommand:
                    command.AddCommand(subCommand);
                    break;
                default:
                    break;
            }
        }
    }

    public class Command : BaseSymbolPart<Command>, ICanParent
    {
        internal Command(string name, string help = default) : base(name, help)
        { }

        public class CommandCollection : IEnumerable<Command>
        {
            private readonly List<Command> _commands = new List<Command>();

            internal TNewCommand AddCommand<TNewCommand>(TNewCommand newCommand)
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

        // I'm wondering whether these are an inherent part of the definition or part of parsing. 
        public bool? TreatUnmatchedTokensAsErrors { get; }
        internal MethodBinder ExecutionHandler { get; }
        public new CommandResult Result => (CommandResult)base.Result;

        public CommandCollection SubCommands { get; } = new CommandCollection();
        public Option.OptionCollection Options { get; } = new Option.OptionCollection();

        internal Command AddCommand(Command newCommand)
        {
            SubCommands.AddCommand(newCommand);
            return this;
        }

        public static Command Create(string name, string help = null)
            => new Command(name, help);

        //public Command AddCommand(string name, string help = null, params BaseSymbolPart[] subParts)
        //    => AddCommand(Create(name, help, subParts));

        public static Command Create(string name, params BasePart[] subParts)
            => new Command(name)
              .AddSubParts(subParts);

        public static Command Create(string name, string help, params BasePart[] subParts)
            => new Command(name, help)
               .AddSubParts(subParts);

        public static Command Create<T>(string name, ArgumentList<T> argument, params BasePart[] subParts)
            => new Command<T>(name,default, default, argument)
              .AddSubParts(subParts);

        public static Command Create<T>(string name, string help, ArgumentList<T> argument, params BasePart[] subParts)
           => new Command<T>(name, help, default, argument)
               .AddSubParts(subParts);

        protected override void AcceptChildren(IVisitor<Command> visitor)
        {
            foreach(var command in SubCommands)
            { command.Accept(visitor); }
            if (visitor is IVisitorStart<Option> optionVisitor )
            foreach (var option in Options)
            { option.Accept(optionVisitor); }
        }

    }

    public class Command<T> : Command, IHasArgument 
    {
        internal Command(string name, string help, string[] aliases, Arity arity)
            : base(name, help)
            =>
            // OK, this is ugly. Get someone to explain why I need this. 
            Argument = ArgumentList.Create<T>(arity: arity);

        internal Command(string name, string help, string[] aliases, ArgumentList<T> argument)
            : base(name, help)
            =>
            // OK, this is ugly. Get someone to explain why I need this. 
            Argument = argument;

        public ArgumentList<T> Argument { get; internal set; }

        public Func<object> DefaultValueFunc => ((IHasArgument)Argument).DefaultValueFunc;

        public object DefaultValue => ((IHasArgument)Argument).DefaultValue;

        public Arity Arity => ((IHasArgument)Argument).Arity;

        ArgumentList IHasArgument.Argument
            => Argument;

        ArgumentResult IHasArgument.Result => ((IHasArgument)Argument).Result;

        protected override void AcceptChildren(IVisitor<Command> visitor)
        {
            if (visitor is IVisitorStart<ArgumentList> argumentVisitor)
            {
                Argument.Accept(argumentVisitor);
            }
            base.AcceptChildren(visitor);
        }

    }


}
