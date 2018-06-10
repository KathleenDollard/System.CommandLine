using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.CommandLine
{
    // KAD: Option questions:
    //   What does "respecified" mean?
    //   Is Token what was actually specified?

    // This base class is used for boolean options
    public class Option : BaseSymbolPart, ICanParent
    {
        internal Option(string name, string help)
            : base(name, help)
        { }

        internal Option(string name, string help, params string[] aliases)
            : base(name, help)
            => Aliases = aliases;

        public class OptionCollection : IEnumerable<Option>
        {
            private readonly List<Option> _options = new List<Option>();

            public Option this[string idOrName]
                => _options.FirstOrDefault(x => x.Id == idOrName)
                   ?? _options.FirstOrDefault(x => x.Name == idOrName)
                   ?? _options.FirstOrDefault(x => x.Aliases.Contains(idOrName));

            internal void AddOption(Option option)
               => _options.Add(option);

            public IEnumerator<Option> GetEnumerator()
                => _options.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        public IEnumerable<string> Aliases { get; private set; }

        public class OptionResult : BaseResult
        {
            public string SpecifiedToken { get; internal set; }
        }

        /// <summary>
        /// Create a boolean option
        /// </summary>
        public static Option Create(string name, string help = default, string[] aliases = default)
            => new Option(name, help);

        /// <summary>
        /// Create an option with a single string argument
        /// </summary>
        public static Option Create(string name, Arity.Single arity, string defaultValue = default)
            => new Option<string, Argument<string>>(name, default, default, arity, defaultValue);

        /// <summary>
        /// Create an option with a single string argument
        /// </summary>
        public static Option Create(string name, string help, Arity.Single arity, string defaultValue = default)
            => new Option<string, Argument<string>>(name, help, default, arity, defaultValue);

        /// <summary>
        /// Create an option with a single string argument
        /// </summary>
        public static Option Create(string name, string help, string[] aliases, Arity.Single arity, string defaultValue = default)
            => new Option<string, Argument<string>>(name, help, aliases, arity, defaultValue);

        /// <summary>
        /// Create an option with a string argument list
        /// </summary>
        public static Option Create(string name, Arity.Many arity)
            => new Option<string, ArgumentList<string>>(name, default, default, arity);

        /// <summary>
        /// Create an option with a string argument list
        /// </summary>
        public static Option Create(string name, string help, Arity.Many arity)
            => new Option<string, ArgumentList<string>>(name, help, default, arity);

        /// <summary>
        /// Create an option with a string argument list
        /// </summary>
        public static Option Create(string name, string help, string[] aliases, Arity.Many arity)
            => new Option<string, ArgumentList<string>>(name, help, aliases, arity);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option Create<T>(string name, string help = default, string[] aliases = default, Arity.Single arity = default, T defaultValue = default)
            => new Option<T, Argument<T>>(name, help, aliases, arity, defaultValue);

        /// <summary>
        /// Create an option with a string argument list of a type that must be specified
        /// </summary>
        public static Option Create<T>(string name, Arity.Many arity)
            => new Option<T, ArgumentList<T>>(name, default, default, arity);

        /// <summary>
        /// Create an option with a string argument list of a type that must be specified
        /// </summary>
        public static Option Create<T>(string name, string help, Arity.Many arity)
            => new Option<T, ArgumentList<T>>(name, help, default, arity);

        /// <summary>
        /// Create an option with a string argument list of a type that must be specified
        /// </summary>
        public static Option Create<T>(string name, string help, string[] aliases, Arity.Many arity)
            => new Option<T, ArgumentList<T>>(name, help, aliases, arity);

        /// <summary>
        /// Create an option with the specified argument
        /// </summary>
        public static Option Create<T>(string name, Argument<T> argument)
            => new Option<T, Argument<T>>(name, default, argument);

        /// <summary>
        /// Create an option with the specified argument
        /// </summary>
        public static Option Create<T>(string name, string help, Argument<T> argument)
            => new Option<T, Argument<T>>(name, help, argument);

        /// <summary>
        /// Create an option with the specified argument
        /// </summary>
        public static Option Create<T>(string name, ArgumentList<T> argument)
            => new Option<T, ArgumentList<T>>(name, default, argument);

        /// <summary>
        /// Create an option with the specified argument
        /// </summary>
        public static Option Create<T>(string name, string help, ArgumentList<T> argument)
            => new Option<T, ArgumentList<T>>(name, help, argument);

    }

    public class Option<T, TArg> : Option
        where TArg : BaseArgument<T>
    {
        public Option(string name, string help = default)
             : base(name, help)
        { }

        internal Option(string name, string help = default, string[] aliases = default, Arity.Many arity = default)
            : base(name, help, aliases)
        {
            // OK, this is ugly. Get someone to explain why I need this. 
            Argument = (TArg)(BaseArgument<T>)System.CommandLine.Argument.Create<T>(arity: arity);
        }

        internal Option(string name, string help, string[] aliases, Arity.Single arity = default,
            T defaultValue = default)
            : base(name, help, aliases)
        {
            Argument = (TArg)(BaseArgument<T>)System.CommandLine.Argument.Create(arity: arity, defaultValue: defaultValue);
        }

        public Option(string name, string help, TArg argument, params string[] aliases)
            : base(name, help, aliases)
        {
            Argument = argument;
        }

        public TArg Argument { get; internal set; }
    }
}
