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

    // Exactly like Command, except different constructor to aid in learning

    public static class CommandExtensions
    {
        internal static TCmd AddArgument<TCmd, T,TArg>(this TCmd command, TArg argument)
            where TCmd : Command<T, TArg>
            where TArg : BaseArgument<T>
        {
            argument.Parent = command;
            command.Argument = argument;
            return command;
        }

        internal static TCmd AddSubParts<TCmd>(this TCmd command, BaseSymbolPart[] subParts)
            where TCmd : Command
        {
            foreach (var part in subParts)
            {
                command.AddPart(part);

            }
            return command;
        }

        internal static void AddPart<TCmd>(this TCmd command, BaseSymbolPart part)
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

    public class CommandLine : Command
    {
        internal CommandLine(string help = null) : base(default, help)
        { }

        public static CommandLine Create(string help = null)
         => new CommandLine(help);

        public static CommandLine Create(params BaseSymbolPart[] subParts)
            => Create(default, subParts);

        public static new CommandLine Create(string help, params BaseSymbolPart[] subParts)
            => Create(help)
               .AddSubParts(subParts);
    }

    public class CommandLine<T, TArg> : Command<T, TArg>
         where TArg : BaseArgument<T>
    {
        public CommandLine(string help = null) : base("", help)
        { }
    }

    public class Command : BaseSymbolPart, ICanParent
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

        public bool? TreatUnmatchedTokensAsErrors { get; }
        internal MethodBinder ExecutionHandler { get; }

        public CommandCollection Commands { get; } = new CommandCollection();
        public Option.OptionCollection Options { get; } = new Option.OptionCollection();

        internal Command AddCommand(Command newCommand)
        {
            Commands.AddCommand(newCommand);
            return this;
        }

        public static Command Create(string name, string help = null)
            => new Command(name, help);

        public Command AddCommand(string name, string help = null, params BaseSymbolPart[] subParts)
            => AddCommand(Create(name, help, subParts));

        public static Command Create(string name, params BaseSymbolPart[] subParts)
            => Create(name, (string)null, subParts);

        public static Command Create(string name, string help, params BaseSymbolPart[] subParts)
            => Create(name, help)
               .AddSubParts(subParts);

        public static Command Create<T>(string name, BaseArgument<T> argument, params BaseSymbolPart[] subParts)
            => Create(name, null, argument, subParts);

        public static Command Create<T>(string name, string help, BaseArgument<T> argument, params BaseSymbolPart[] subParts)
            => Create(name, help)
               .AddSubParts(subParts)
               .AddArgument(argument);

        public class CommandResult : BaseResult
        {
            public string SpecifiedToken { get; internal set; }
            public CommandCollection CalledCommands { get; } = new CommandCollection();
            public Option.OptionCollection CalledOptions { get; } = new Option.OptionCollection();
        }
    }

    public class Command<T, TArg> : Command, IHasArgument
        where TArg : BaseArgument<T>
    {
        internal Command(string name, string help = default)
            : base(name, help)
        { }

        internal Command(string name, string help = default, string[] aliases = default, Arity.Many arity = default)
            : base(name, help)
            =>
            // OK, this is ugly. Get someone to explain why I need this. 
            Argument = (TArg)(BaseArgument<T>)System.CommandLine.Argument.Create<T>(arity: arity);

        public TArg Argument { get; internal set; }

        public BaseArgument BaseArgument
            => Argument;
    }


}
