using System.Collections.Generic;
using KAD = System.CommandLine;
using System.Linq;
using System.Collections.ObjectModel;

namespace System.CommandLine.Parser
{
    public class ParserWrapper
    {

        public static CommandDefinition CreateDefinition(KAD.Command command)
        {
            if (command.Name == null)
            {
                command.Name = ParserUtils.ExeName.Value;
            }
            return CommanDefinitionFrom(command);
        }

        public static void StoreResults (CommandLine commandLine, ParseResult parseResult)
        {

        }

        private static CommandDefinition CommanDefinitionFrom(KAD.Command command)
        {
            var symbolCollection = SymbolDefinitonFrom(command.Options).Union(SymbolDefinitionFrom(command.SubCommands));
            var commandDefiniton = new CommandDefinition(command.Name, command.Help,
                    new ReadOnlyCollection<SymbolDefinition>(symbolCollection.ToList()),
                    ArgumentDefinitionFrom(command),
                    command.TreatUnmatchedTokensAsErrors.HasValue
                        ? command.TreatUnmatchedTokensAsErrors.Value
                        : true,
                    command.ExecutionHandler);
            return commandDefiniton;

        }

        private static ArgumentDefinition ArgumentDefinitionFrom(object obj)
            => obj is IHasArgument withArgument
               ? ArgumentDefinitionFrom(withArgument)
               : null;

        private static ArgumentDefinition ArgumentDefinitionFrom(IHasArgument withArgument)
        {
            Func<object> defaultValueFunc = null;
            if (withArgument.DefaultValueFunc != null)
            {
                defaultValueFunc= withArgument.DefaultValueFunc;
                if (withArgument.DefaultValue != null)
                {
                    defaultValueFunc = () => withArgument.DefaultValue;
                }
            }
            //Func<object> defaultValueFunc = withArgument.DefaultValueFunc
            //                                ??(withArgument.DefaultValue == null
            //                                      ? (Func<object>)null
            //                                      : () => withArgument.DefaultValue);

            var argumentParser = new ArgumentParser(ArityValidatorFrom(withArgument.Arity));
            // TODO: Several things missing
            return new ArgumentDefinition(argumentParser, defaultValueFunc);
        }

        private static ArgumentArityValidator ArityValidatorFrom(Arity arity)
        {

            if (arity == Arity.Zero)
            { return ArgumentArity.Zero; }
            if (arity == Arity.ZeroOrOne)
            { return ArgumentArity.ZeroOrOne; }
            if (arity == Arity.ExactlyOne)
            { return ArgumentArity.ExactlyOne; }
            if (arity == Arity.ZeroOrMore)
            { return ArgumentArity.ZeroOrMore; }
            if (arity == Arity.OneOrMore)
            { return ArgumentArity.OneOrMore; }
            return null;
        }

        private static IEnumerable<SymbolDefinition> SymbolDefinitionFrom(KAD.Command.CommandCollection commands)
        {
            var list = new List<CommandDefinition>();
            foreach (var command in commands)
            {
                list.Add(CommanDefinitionFrom(command));
            }
            return list;
        }

        private static IEnumerable<SymbolDefinition> SymbolDefinitonFrom(KAD.Option.OptionCollection options)
        {
            var list = new List<OptionDefinition>();
            foreach (var option in options)
            {
                list.Add(OptionDefinitionFrom(option));
            }
            return list;
        }

        private static OptionDefinition OptionDefinitionFrom(KAD.Option option)
        {
            var aliases = new List<string>() { EnsureStartsWith("--", option.Name) };
            if (option.Aliases != null)
            {
                aliases.AddRange(option.Aliases.Select(x => EnsureStartsWith("-", x)));
            }
            var optionDefinition = new OptionDefinition(aliases, option.Help, ArgumentDefinitionFrom(option));
            return optionDefinition;
        }

        private static string EnsureStartsWith(string prefix, string name)
            => name.StartsWith(prefix)
               ? name
               : prefix + name;
    }
}
