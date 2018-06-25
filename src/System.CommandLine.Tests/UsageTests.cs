using System.CommandLine.Formatters;
using System.CommandLine.Parser;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace System.CommandLine.Tests
{
    public class UsageTests
    {
     

        private readonly CommandLine _command =
          CommandLine.Create("",
             commands: new Command[] {
                 Command.Create("the-command", "help!",
                      argument: Argument.Create(arity: Arity.OneOrMore, "arg1"),
                      options: new[] {
                             Option.Create("opt1"),
                             Option.Create<int?>("opt2","help me!"),
                             Option.Create<int?>( "opt3"),
                             Option.Create("opt4", 
                                 argument: Argument.Create(arity: Arity.ExactlyOne, "opt_arg1", defaultValue: 0)),
                             Option.Create("opt5", "",
                                 argument: Argument.Create<int?>(arity: Arity.ZeroOrMore, "opt_arg2")) },
                      commands: new[] {
                             Command.Create("subCmd1"),
                             Command.Create("subCmd2",
                                 options: new [] {Option.Create<int?>("subCmd2-opt2", "help me!") })
                      }),
                 Command.Create("cmd2", "", Argument.Create(arity: Arity.ZeroOrOne, "arg2")),
                 Command.Create("cmd3")
            });

        [Fact]
        public void BasicUsage()
        {
            var formatter = new TestFormatter()
                .Parse(_command);

        }

        [Fact]
        public void VisitorReport()
        {
            var visitor = new ReportVisitor();
            var report = visitor.Report(_command);
        }

        [Theory]
        [InlineData("the-command --opt2", "[ testhost ![ the-command [ --opt2 ] *[ --opt4 <0> ] ] ]")]
        [InlineData("the-command --opt2 alpha beta", "[ testhost ![ the-command [ --opt2 <alpha> ] *[ --opt4 <0> ] <beta> ] ]")]
        [InlineData("the-command alpha beta", "[ testhost ![ the-command *[ --opt4 <0> ] <alpha> <beta> ] ]")]
        [InlineData("the-command --opt3 Fred", "[ testhost ![ the-command [ --opt3 <Fred> ] *[ --opt4 <0> ] ] ]")]
        [InlineData("the-command --opt2 --opt3 Fred alpha beta", "[ testhost ![ the-command [ --opt2 <alpha> ] [ --opt3 <Fred> ] *[ --opt4 <0> ] <beta> ] ]")]
        public void ParseSample(string input, string expected)
        {
            var commandDefinition = ParserWrapper.CreateDefinition(_command);
            // I don't like this semantics or extra step. I think the API creates the definition
            var parser = new System.CommandLine.Parser.Parser(commandDefinition);
            var diagram = parser.Parse(input).Diagram();
            diagram.Should().BeEquivalentTo(expected);
        }

        // Results noodling
        //private void ResultsNoodling()
        //{
        //    CommandLineResult result = ParserWrapper.Parse(_command);
        //    Command command = result.Command; // Final SubCommand 
        //    IEnumerable<OptionResult> options = result.Options;
        //    IEnumerable<OptionResult> allOptions = result.AllOptions; // collapses options from parents. Do we want that?
        //    IEnumerable<T> arguments = result.GetValues();
        //    T argument = result.GetValue();
        //    CommandResult parent = result.CommandResultTree;


        //}


    }
}
