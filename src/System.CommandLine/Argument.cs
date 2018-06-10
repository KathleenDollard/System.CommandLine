using System.Collections.Generic;

namespace System.CommandLine
{
    // Argument questions:



    public abstract class Argument
    {
        public static Argument<string> Create(string name = default, string help = default, Arity.Single arity = default, string defaultValue = default)
            => new Argument<string>(name, help, arity, defaultValue);

        public static ArgumentList<string> Create(string name = default, string help = default, Arity.Many arity = default)
           => new ArgumentList<string>(name, help, arity);

        public static Argument<T> Create<T>(string name = default, string help = default, Arity.Single arity = default, T defaultValue = default)
            => new Argument<T>(name, help, arity, defaultValue);

        public static ArgumentList<T> Create<T>(string name = default, string help = default, Arity.Many arity = default)
             => new ArgumentList<T>(name, help, arity);

    }

    // This exists only to provide polymorphism. Can't use covariance because in/out
    public abstract class BaseArgument : BasePart
    {
        internal BaseArgument(string name, string help)
            : base(name, help)
        { }

        public Arity Arity { get; private protected set; }

    }

    public abstract class BaseArgument<T> : BaseArgument
    {
        internal BaseArgument(string name = null, string help = null)
        : base(name, help) { }
    }

    public class Argument<T> : BaseArgument<T>
    {
        internal Argument(string name = null, string help = null,
            Arity.Single arity = default, T defaultValue = default)
            : base(name, help)
            => Arity = arity;

        public T Value { get; internal set; }
        public T DefaultValue { get; internal set; }
        public Func<T> DefaultValueFunc { get; internal set; }        // At least to allow today
        public new Arity.Single Arity
        {
            get => (Arity.Single)base.Arity;
            internal set => base.Arity = Arity;
        }
        public IEnumerable<Func<T, ValidationIssue>> Validators { get; } = new List<Func<T, ValidationIssue>>();
    }

    public class ArgumentList<T> : BaseArgument<T>
    {
        internal ArgumentList(string name = null, string help = null,
            Arity.Many arity = default)
            : base(name, help)
            => Arity = arity;

        public IEnumerable<T> Values { get; }
        public new Arity.Many Arity
        {
            get => (Arity.Many)base.Arity;
            internal set => base.Arity = Arity;
        }
        public IEnumerable<Func<T, ValidationIssue>> Validators { get; } = new List<Func<T, ValidationIssue>>();
    }
}
