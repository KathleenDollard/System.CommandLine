using System.Collections.Generic;
using System.CommandLine.Common;

namespace System.CommandLine.Result
{
    public abstract class BaseResult
    {
        public IEnumerable<ValidationIssue> ValidationIssues { get; internal set; }

        // TODO: Ensure this makes sense on arguments
        public bool IsUsed { get; internal set; }

        // Something needed this, makes no sense
        public int Count;
    }
}
