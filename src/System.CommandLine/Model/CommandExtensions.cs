using System.Collections.Generic;

namespace System.CommandLine
{
    public static class CommandExtensions
    {
        internal static Command<T> AddArgument<TCmd, T>(this Command<T> command, Argument<T> argument)
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

        public static Command<T> WithArgumentList<T>(this Command<T> command, Argument<T> argument)
        {
            command.Argument = argument;
            return command;
        }

        public static Command<T> WithArgumentList<T>(this Command<T> option, string name, Arity arity = default, T defaultValue = default)
        {
            var argument = new Argument<T>(name, arity: arity) {
                DefaultValue = defaultValue
            };
            option.Argument = argument;
            return option;
        }

        public static TArg WithArity<TArg>(this TArg argument,Arity arity)
            where TArg : Argument
        {
            argument.Arity = arity;
            return argument;
        }

    }


}
