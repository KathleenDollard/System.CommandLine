namespace System.CommandLine
{
    public interface IVisitable<TPart>
        //where TPart: BasePart<TPart>
    {
        void AcceptThisOnly(IVisitor<TPart> visitor);
        void Accept(IVisitor<TPart> visitor);
    }
}
