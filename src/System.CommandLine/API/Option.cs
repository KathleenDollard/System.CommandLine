using System;
using System.Collections.Generic;
using System.CommandLine.Common;
using System.CommandLine.Result;
using System.Text;

namespace System.CommandLine.API
{

    public static class OptionExtensions
    {

        public static TOpt WithArgumentList<TOpt, T>(this TOpt option, Argument<T> argument)
            where TOpt : Option<T>
        {
            option.Argument = argument;
            return option;
        }


        public static Option<T> WithArgumentList< T>(this Option<T> option, string name, Arity arity = default, T defaultValue = default)
        {
            var argument = new Argument<T>(name, arity: arity) {
                DefaultValue = defaultValue
            };
            option.Argument = argument;
            return option;
        }
    }

    // KAD: Option questions:
    //   What does "respecified" mean?
    //   Is Token what was actually specified?

    // This base class is used for boolean options
    public partial class Option : BaseSymbolPart<Option>, ICanParent
    {
        internal Option(string name, string help)
            : base(name, help)
        { }

        internal Option(string name, string help, params string[] aliases)
            : base(name, help)
            => Aliases = aliases;

        public new OptionResult Result => (OptionResult)base.Result;

        public IEnumerable<string> Aliases { get; } = new List<string>();

        /// <summary>
        /// Create a boolean option
        /// </summary>
        public static Option Create(string name)
            => new Option(name, null);

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
        public static Option<T> Create<T>(string name)
            => new Option<T>(name, default);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option<T> Create<T>(Arity arity, string name, string help = default, string[] aliases = default, T defaultValue = default)
            => new Option<T>(name, help, aliases, arity, defaultValue);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option<T> Create<T>(string name, string help = default, string[] aliases = default, T defaultValue = default)
            => new Option<T>(name, help, aliases, default, defaultValue);

        /// <summary>
        /// Create a boolean option
        /// </summary>
        public static Option<T> Create<T>(string name, string help = default)
            => new Option<T>(name, help, default, (Arity)default);

        /// <summary>
        /// Create an option with a single argument of a type that can be specified or inferred from the default value
        /// </summary>
        public static Option<T> Create<T>(string name, string help = default, string[] aliases = default, T defaultValue = default, Argument<T> argument = default)
            => new Option<T>(name, help, argument, aliases);


    }

    public class Option<T> : Option, IHasArgument
    {
        public Option(string name)
            : this(name, default, default, (Arity)default, default)
        { }

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

            Argument = API.Argument.Create<T>(arity: arity);
        }

        public Option(string name, string help, Argument<T> argument, params string[] aliases)
            : base(name, help, aliases)
        {
            Argument = argument;
        }

        public Argument<T> Argument { get; internal set; }

        Argument IHasArgument.Argument => Argument;

        protected override void AcceptChildren(IVisitor<Option> visitor)
        {
            if (visitor is IVisitorStart<Argument> argumentVisitor)
            {
                Argument.Accept(argumentVisitor);
            }
        }
    }
}
