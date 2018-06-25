namespace System.CommandLine.Common
{
    public interface IVisitable<TPart>
        //where TPart: BasePart<TPart>
    {
        void AcceptThisOnly(IVisitor<TPart> visitor);
        void Accept(IVisitor<TPart> visitor);
    }
}
