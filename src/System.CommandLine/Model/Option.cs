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
    public class Option : BaseSymbolPart<Option>, ICanParent
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

        public new OptionResult Result => (OptionResult)base.Result;

        public IEnumerable<string> Aliases { get; } = new List<string>();

        /// <summary>
        /// Create a boolean option
        /// </summary>
        public static Option Create(string name, string help = default, string[] aliases = default)
            => new Option(name, help);

        /// <summary>
        /// Create an option with string argument(s)
        /// </summary>
        public static Option<string> Create(Arity arity, string name, string help = default, string[] aliases = default, string defaultValue = default)
            => new Option<string>(name, help, aliases, arity, defaultValue);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option Create<T>(Arity arity, string name, string help = default, string[] aliases = default, T defaultValue = default)
            => new Option<T>(name, help, aliases, arity, defaultValue);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option Create<T>( string name, string help = default, string[] aliases = default, T defaultValue = default)
            => new Option<T>(name, help, aliases, default, defaultValue);

        /// <summary>
        /// Create a boolean option
        /// </summary>
        public static Option Create<T>(string name, string help = default)
            => new Option<T>(name, help, default,(Arity)default);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option Create<T>(string name, string help = default, string[] aliases = default, T defaultValue = default, ArgumentList<T> argument = default)
            => new Option<T>(name, help, argument, aliases);

    }

    public class Option<T> : Option , IHasArgument
    {
        public Option(string name, string help = default)
             : this(name, help, default, (Arity)default, default)
        { }

        internal Option(string name, string help = default, string[] aliases = default, Arity arity = default, T defaultValue = default)
            : base(name, help, aliases)
        {
            // OK, this is ugly. Get someone to explain why I need this.
            arity = arity == default
                    ? Arity.ZeroOrMore
                    : arity;

            Argument = ArgumentList.Create<T>(arity: arity);
        }

        public Option(string name, string help, ArgumentList<T> argument, params string[] aliases)
            : base(name, help, aliases)
        {
            Argument = argument;
        }

        public ArgumentList<T> Argument { get; internal set; }

        ArgumentList IHasArgument.Argument => ((IHasArgument)Argument).Argument;

        public Func<object> DefaultValueFunc => ((IHasArgument)Argument).DefaultValueFunc;

        public object DefaultValue => ((IHasArgument)Argument).DefaultValue;

        public Arity Arity => ((IHasArgument)Argument).Arity;

        ArgumentResult IHasArgument.Result => ((IHasArgument)Argument).Result;

        protected override void AcceptChildren(IVisitor<Option> visitor)
        {
            if (visitor is IVisitorStart<ArgumentList> argumentVisitor)
            {
                Argument.Accept(argumentVisitor);
            }
        }
    }
}
