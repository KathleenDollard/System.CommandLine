using System.CommandLine.Formatters;
using Xunit;

namespace System.CommandLine.Tests
{
    public class UsageTests
    {
        private readonly Command _command =
            CommandLine.Create("",
               Command.Create("the-command", "help!",
                    Argument.Create("arg1", arity: Arity.OneOrMore),
                    Option.Create("opt1"),
                    Option.Create<int>("opt2", "help me!"),
                    Option.Create<int>("opt3", "", arity: Arity.ExactlyOne),
                    Option.Create("opt4", "", Argument.Create("opt_arg", arity: Arity.ExactlyOne, defaultValue: 0)),
                    Option.Create("opt5", "", Argument.Create<int>("opt_arg", arity: Arity.ZeroOrMore)),
                    Command.Create("subCmd1"),
                    Command.Create("subCmd2", Option.Create<int>("opt2", "help me!"))
                    ),
               Command.Create("cmd2", Argument.Create("arg2", arity: Arity.ZeroOrOne)),
               Command.Create("cmd3"));

        [Fact]
        public void BasicUsage()
        {
            var formatter = new TestFormatter()
                .Parse(_command);

        }


    }
}
