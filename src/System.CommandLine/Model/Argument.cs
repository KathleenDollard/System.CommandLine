using System.Collections.Generic;

namespace System.CommandLine
{
    // Argument questions:




    public abstract class ArgumentList : BasePart<ArgumentList>
    {

        internal ArgumentList(string name, string help)
               : base(name, help)
        { }

        public static ArgumentList<string> Create(Arity arity = default, string name = default,
                    string help = default, string defaultValue = default)
           => new ArgumentList<string>(name, help, arity);

        public static ArgumentList<T> Create<T>(Arity arity = default, string name = default,
                    string help = default, T defaultValue = default)
           => new ArgumentList<T>(name, help, arity);

        public Arity Arity { get; private protected set; }
        public new ArgumentResult Result => (ArgumentResult)base.Result;


    }

    public class ArgumentList<T> : ArgumentList, IHasArgument
    {
        internal ArgumentList(string name = null, string help = null,
            Arity arity = default)
            : base(name, help)
            => Arity = arity;

        public T DefaultValue { get; internal set; }
        public Func<T> DefaultValuesFunc { get; internal set; }        // At least to allow today
        private ArgumentResult<T> SpecificResult { get; } = new ArgumentResult<T>();
        public IEnumerable<Func<T, ValidationIssue>> Validators { get; } = new List<Func<T, ValidationIssue>>();

        ArgumentList IHasArgument.Argument => throw new NotImplementedException();

        Func<object> IHasArgument.DefaultValueFunc => DefaultValuesFunc == null
                                                      ? (Func<object>)null
                                                      : () => DefaultValuesFunc();

        object IHasArgument.DefaultValue => DefaultValue;

        string ISymbol.Token => throw new NotImplementedException();
    }
}
