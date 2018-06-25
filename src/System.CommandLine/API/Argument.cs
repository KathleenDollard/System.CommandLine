using System.Collections.Generic;

namespace System.CommandLine.API
{
    // Argument questions:


    public abstract class Argument : BasePart<Argument>
    {

        internal Argument(string name)
             : base(name, default)
        { }

        internal Argument(string name, string help)
               : base(name, help)
        { }

        public static Argument<string> Create(string name = default,
             string help = default, string defaultValue = default)
            => new Argument<string>(name, help, default);

        public static Argument<string> Create(Arity arity = default, string name = default,
                    string help = default, string defaultValue = default)
           => new Argument<string>(name, help, arity);

        public static Argument<T> Create<T>(string name = default,
                    string help = default, T defaultValue = default)
           => new Argument<T>(name, help, default);

        public static Argument<T> Create<T>(Arity arity = default, string name = default,
                    string help = default, T defaultValue = default)
           => new Argument<T>(name, help, arity);

        public Arity Arity { get; internal set; }
        //public new ArgumentResult Result => (ArgumentResult)base.Result;

        protected abstract object GetDefaultValue();
        public object DefaultValue
            => GetDefaultValue();
        protected abstract Func<object> GetDefaultValueFunc();
        public Func<object> DefaultValueFunc
            => GetDefaultValueFunc();

    }

    public class Argument<T> : Argument
    {
        internal Argument(string name = default, string help = default,
            Arity arity = default)
            : base(name, help)
            => Arity = arity;

        internal static Argument<T> MakeArgument(string name = default, string help = default,
            Arity arity = default, T defaultValue = default)
        {

            // OK, this is ugly. Get someone to explain why I need the full name. 
            var argument = System.CommandLine.API.Argument.Create<T>(
                arity: arity ?? Arity.ZeroOrMore,
                name: name,
                help: help,
                defaultValue: defaultValue);
            return argument;
        }

        protected override object GetDefaultValue()
            => DefaultValue;
        // The object cast is required
        protected override Func<object> GetDefaultValueFunc()
            => DefaultValuesFunc == null
                ? (Func<object>)null
                : () => (object)DefaultValuesFunc();

        public new T DefaultValue { get; internal set; }
        public Func<T> DefaultValuesFunc { get; internal set; }   // At least to allow today
    }
}
