using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine
{
    public abstract class BaseResult
    {
        public IEnumerable<ValidationIssue> ValidationIssues { get; internal set; }

        // TODO: Ensure this makes sense on arguments
        public bool IsUsed { get; internal set; }

        // Something needed this, makes no sense
        public int Count;
    }


    public class CommandLineResult : BaseResult
    {
        public Command Command { get; internal set; }
    }

    public class ArgumentResult : BaseResult
    {
    }

    public class ArgumentResult<T>: ArgumentResult
    {
        public IEnumerable<T> Values { get; }
        public T Value { get; internal set; }

    }

    public class OptionResult : BaseResult
    {
        public bool IsImplicit { get; set; }
    }

    public class OptionResult<T> : OptionResult
    {
        public T TypedValue { get; internal set; }
    }

    public class CommandResult : BaseResult
    {
    }

    public class CommandResult<T> : CommandResult
    {
    }

}
