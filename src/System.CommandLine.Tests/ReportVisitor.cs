using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine.Tests
{
    public class ReportVisitor : IVisitorStart<Command>, IVisitorEnd<Command>,
                                 IVisitorStart<Option>,
                                 IVisitorStart<Argument>
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indent = 0;

        private string Spaces(int count)
                => new string(' ', count);

        public void Visit(Command command)
        {
            // The children should be indented, but not this one
            _indent += 5;
            _sb.AppendLine($"{Spaces(_indent - 5)}command:  {command.Name}");
        }
        public void VisitEnd(Command command)
        {
            _indent -= 5;
        }

        public void Visit(Option option)
            => _sb.AppendLine($"{Spaces(_indent)}option:   {option.Name}");
        public void Visit(Argument argument)
            => _sb.AppendLine($"{Spaces(_indent)}argument: {argument.Name ?? "<No Name Given>"}");
        internal object Report<TCmd>(TCmd command)
            where TCmd : BaseCommand

        {
            command.Accept(this);
            return _sb.ToString();
        }
    }
}
