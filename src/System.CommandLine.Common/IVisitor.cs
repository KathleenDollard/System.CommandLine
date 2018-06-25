namespace System.CommandLine.Common
{
    //public interface IVisitor
    //{
    //    void Visit<T>(T item);
    //}

    public interface IVisitor<T> // Cause people expect it
    {
        void Visit(T item);
    }

    public interface IVisitorStart<T> : IVisitor<T> // For clarity
    {
    }

    public interface IVisitorEnd<T> : IVisitor<T>
    {
        void VisitEnd(T item);
    }

}
