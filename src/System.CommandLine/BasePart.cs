using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine
{
    // KAD: TODO: Skipping Suggest for now, was on SymbolDefinition and ArgumentDefinition
    public abstract class BasePart
    {
        protected static readonly string _idSeperator = ".";

        public BasePart(string name, string help = "")
        {
            Name = name;
            Help = help;
        }

        public string Id { get; internal set; }
        internal ICanParent Parent { get; set; }

        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public override string ToString() => $"{GetType().Name}: {Name}";
    }

    public abstract class BaseSymbolPart : BasePart
    {
        public BaseSymbolPart(string name, string help = "")
            : base(name, help)
        {  }
    }

    public abstract class BaseResult
    {
        public IEnumerable<ValidationIssue> ValidationIssues { get; internal set; }
        // TODO: Ensure this makes sense on arguments
        public bool IsCalled { get; internal set; }
    }
}
