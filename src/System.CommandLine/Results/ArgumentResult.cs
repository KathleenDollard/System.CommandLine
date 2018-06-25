using System.Collections.Generic;

namespace System.CommandLine.Result
{
    public class ArgumentResult : BaseResult
    {
        public IEnumerable<object> Values { get; }
    }

    public class ArgumentResult<T> : ArgumentResult
    {
        public new IEnumerable<T> Values { get; }
        public T Value { get; internal set; }

    }
}
