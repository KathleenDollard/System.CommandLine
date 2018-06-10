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
        public ICanParent Parent { get; set; }
        public BaseResult Result { get; private protected set; }

        public string Name { get; internal set; }
        public string Help { get; internal set; }
        public override string ToString() => $"{GetType().Name}: {Name}";
    }

    public abstract class BasePart<TPart> : BasePart, IVisitable<TPart>
        where TPart : BasePart<TPart>
    {
        public BasePart(string name, string help = "")
              : base(name, help)
        { }
        protected virtual void AcceptChildren(IVisitor<TPart> visitor)
        { }

        public void AcceptThisOnly(IVisitor<TPart> visitor)
            => visitor.Visit((TPart)this);

        public void Accept(IVisitor<TPart> visitor)
        {
            visitor.Visit((TPart)this);
            AcceptChildren(visitor);
            if (visitor is IVisitorEnd<TPart> visitorEnd)
            {
                visitorEnd.VisitEnd((TPart)this);
            }
        }
    }

    public abstract class BaseSymbolPart<TPart> : BasePart<TPart>, ISymbol
       where TPart : BasePart<TPart>
    {
        public BaseSymbolPart(string name, string help = "")
            : base(name, help)
        { }

        public string Token => Name;
    }

    public class ParseInfo
    {

    }
}
