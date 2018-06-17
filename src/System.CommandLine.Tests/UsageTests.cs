using System.CommandLine.Formatters;
using System.CommandLine.Parser;
using FluentAssertions;
using Xunit;

namespace System.CommandLine.Tests
{
    public class UsageTests
    {
        private readonly Command _command =
            CommandLine.Create("",
               Command.Create("the-command", "help!",
                    ArgumentList.Create(arity: Arity.OneOrMore, "arg1"),
                    Option.Create("opt1"),
                    Option.Create<int?>("opt2", "help me!"),
                    Option.Create<int?>(arity: Arity.ExactlyOne, "opt3", ""),
                    Option.Create("opt4", "", argument: ArgumentList.Create(arity: Arity.ExactlyOne, "opt_arg1", defaultValue: 0)),
                    Option.Create("opt5", "", argument: ArgumentList.Create<int?>(arity: Arity.ZeroOrMore, "opt_arg2")),
                    Command.Create("subCmd1"),
                    Command.Create("subCmd2", Option.Create<int?>("subCmd2-opt2", "help me!"))
                    ),
               Command.Create("cmd2", ArgumentList.Create(arity: Arity.ZeroOrOne, "arg2")),
               Command.Create("cmd3"));

        [Fact]
        public void BasicUsage()
        {
            var formatter = new TestFormatter()
                .Parse(_command);

        }

        [Fact]
        public void VisitorReport()
        {
            var visitor = new ReportVis
                itor();
            var report = visitor.Report(_command);
        }

        [Theory]
        [InlineData("the-command --opt2", "[ testhost ![ the-command [ --opt2 ] ] ]")]
        [InlineData("the-command --opt2 alpha beta", "[ testhost ![ the-command [ --opt2 <alpha> ] <beta> ] ]")]
        [InlineData("the-command alpha beta", "[ testhost ![ the-command <alpha> <beta> ] ]")]
        [InlineData("the-command --opt3 Fred", "[ testhost ![ the-command [ --opt3 <Fred> ] ] ]")]
        [InlineData("the-command --opt2 --opt3 Fred alpha beta", "[ testhost ![ the-command [ --opt2 <alpha> ] [ --opt3 <Fred> ] <beta> ] ]")]
        public void ParseSample(string input, string expected)
        {
            var commandDefinition = ParserWrapper.CreateDefinition(_command);
            var parser = new System.CommandLine.Parser.Parser(commandDefinition);
            var diagram = parser.Parse(input).Diagram();
            diagram.Should().BeEquivalentTo(expected);
        }


    }
}
