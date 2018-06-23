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

        // Similar to CommandExtensions but allowing command line return values
    //public static class CommandLineExtensions
    //{
    //    internal static CommandLine<T> AddArgument<TCmd, T>(this CommandLine<T> command, ArgumentList<T> argument)
    //        where TCmd : CommandLine<T>
    //    {
    //        argument.Parent = command;
    //        command.Argument = argument;
    //        return command;
    //    }

    //    internal static TCmd AddSubParts<TCmd>(this TCmd command, IEnumerable<BasePart> subParts)
    //        where TCmd : CommandLine
    //    {
    //        foreach (var part in subParts)
    //        {
    //            command.AddPart(part);

    //        }
    //        return command;
    //    }

    //    internal static void AddPart<TCmd>(this TCmd command, BasePart part)
    //        where TCmd : CommandLine
    //    {
    //        part.Parent = command;
    //        switch (part)
    //        {
    //            case Option option:
    //                command.AddOptions(option);
    //                break;
    //            case Command subCommand:
    //                command.AddCommands(subCommand);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    public static TCmd WithOptions<TCmd>(this TCmd command, params Option[] options)
    //        where TCmd : CommandLine
    //    {
    //        command.AddOptions(options);
    //        return command;
    //    }


    //    public static TCmd WithCommands<TCmd>(this TCmd command, params Command[] childCommands)
    //        where TCmd : CommandLine
    //    {
    //        command.AddCommands(childCommands);
    //        return command;
    //    }

    //    public static TCmd WithHelp<TCmd>(this TCmd part, string help)
    //        where TCmd : CommandLine
    //    {
    //        part.Help = help;
    //        return part;
    //    }

    //    public static Command<T> WithArgumentList<T>(this Command<T> command, ArgumentList<T> argument)
    //    {
    //        command.Argument = argument;
    //        return command;
    //    }

    //    public static Command<T> WithArgumentList<T>(this Command<T> option, string name, Arity arity = default, T defaultValue = default)
    //    {
    //        var argument = new ArgumentList<T>(name, arity: arity) {
    //            DefaultValue = defaultValue
    //        };
    //        option.Argument = argument;
    //        return option;
    //    }

    //    public static TArg WithArity<TArg>(this TArg argument, Arity arity)
    //        where TArg : ArgumentList
    //    {
    //        argument.Arity = arity;
    //        return argument;
    //    }

    //}

    public static class PartExtensions
    {

        public static TPart WithHelp<TPart>(this TPart part, string help)
            where TPart : BasePart
        {
            part.Help = help;
            return part;
        }
    }

    public static class CommandExtensions
    {
        internal static Command<T> AddArgument<TCmd, T>(this Command<T> command, ArgumentList<T> argument)
            where TCmd : Command<T>
        {
            argument.Parent = command;
            command.Argument = argument;
            return command;
        }

        internal static TCmd AddSubParts<TCmd>(this TCmd command, IEnumerable<BasePart> subParts)
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
                    command.AddOptions(option);
                    break;
                case Command subCommand:
                    command.AddCommands(subCommand);
                    break;
                default:
                    break;
            }
        }

        public static TCmd WithOptions<TCmd>(this TCmd command, params Option[] options)
            where TCmd : Command
        {
            command.AddOptions(options);
            return command;
        }


        public static TCmd WithCommands<TCmd>(this TCmd command, params Command[] childCommands)
            where TCmd : Command
        {
            command.AddCommands(childCommands);
            return command;
        }

        public static TCmd WithHelp<TCmd>(this TCmd part, string help)
            where TCmd : CommandLine
        {
            part.Help = help;
            return part;
        }

        public static Command<T> WithArgumentList<T>(this Command<T> command, ArgumentList<T> argument)
        {
            command.Argument = argument;
            return command;
        }

        public static Command<T> WithArgumentList<T>(this Command<T> option, string name, Arity arity = default, T defaultValue = default)
        {
            var argument = new ArgumentList<T>(name, arity: arity) {
                DefaultValue = defaultValue
            };
            option.Argument = argument;
            return option;
        }

        public static TArg WithArity<TArg>(this TArg argument,Arity arity)
            where TArg : ArgumentList
        {
            argument.Arity = arity;
            return argument;
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

        internal Command AddCommands(params Command[] childCommands)
        {
            foreach (var childCommand in childCommands)
            {
                SubCommands.AddCommand(childCommand);
            }
            return this;
        }

        internal Command AddOptions(params Option[] options)
        {
            foreach (var option in options)
            {
                Options.AddOption(option);
            }
            return this;
        }

        public static Command Create(string name)
            => new Command(name, null);

        public static Command<T> Create<T>(string name, string help = null,
                  ArgumentList<T> argument = null,
                  params BasePart[] baseParts)
                  => new Command<T>(name, help)
                     .WithArgumentList(argument)
                     .AddSubParts(baseParts);

        public static Command Create(string name, string help = null,
                  params BasePart[] baseParts)
                  => new Command(name, help)
                     .AddSubParts(baseParts);

        public static Command<T> Create<T>(string name, string help = null,
              ArgumentList<T> argument = null,
              IEnumerable<Command> commands = null,
              IEnumerable<Option> options = null)
              => new Command<T>(name, help)
                 .WithArgumentList(argument)
                 .AddSubParts(commands)
                 .AddSubParts(options);

        public static Command Create(string name, string help = null,
             IEnumerable<Command> commands = null,
             IEnumerable<Option> options = null)
             => new Command(name, help)
                .AddSubParts(commands)
                .AddSubParts(options);

        public static Command Create(string name, string help = null)
            => new Command(name, help);


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
        internal Command(string name, string help, string[] aliases = null, Arity arity = null)
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
