namespace System.CommandLine.Common
{

    public static class VisitorExtensions
    {
        public static IVisitor<T> MakeVisitor<T>(this Action<T> action)
            => new GenericVisitor<T>(action);
     }

    public class GenericVisitor<T1> : IVisitor<T1>
    {
        private readonly Action<T1> _action1;
        public GenericVisitor(Action<T1> action1)
        {
            _action1 = action1;
        }
        public void Visit(T1 item) => _action1(item);
    }

    // TODO: Talk to someone about whether we can fix this and add a bunch of overloads. 
    //public class GenericVisitor<T1, T2> : IVisitor<T1>, IVisitor<T2>
    //{
    //    private readonly Action<T1> _action1;
    //    private readonly Action<T2> _action2;
    //    public GenericVisitor(Action<T1> action1, Action<T2> action2)
    //    {
    //        _action1 = action1;
    //        _action2 = action2;
    //    }
    //    public void Visit(T1 item) => _action1(item);
    //    public void Visit(T2 item) => _action2(item);
    //}

    ////Also can't do this..
    //public interface IVisitor<T1, T2> : IVisitor<T1>, IVisitor<T2>
    //{
    //    void Visit(T1 item);
    //    void Visit(T2 item);
    //}


 
}
