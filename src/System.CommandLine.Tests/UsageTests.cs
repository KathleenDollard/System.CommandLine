using System.CommandLine.Formatters;
using System.CommandLine.Parser;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace System.CommandLine.Tests
{
    public class UsageTests
    {
        private readonly Command _command3 =
          CommandLine.Create("",
             commands: new[] {
                 Command.Create("the-command", "help!",
                      argument: ArgumentList.Create(arity: Arity.OneOrMore, "arg1"),
                      options: new[] {
                             Option.Create("opt1"),
                             Option.Create<int?>("opt2","help me!"),
                             Option.Create<int?>( "opt3", ""),
                             Option.Create("opt4", "",
                                 argument: ArgumentList.Create(arity: Arity.ExactlyOne, "opt_arg1", defaultValue: 0)),
                             Option.Create("opt5", "",
                                 argument: ArgumentList.Create<int?>(arity: Arity.ZeroOrMore, "opt_arg2")) },
                      commands: new[] {
                             Command.Create("subCmd1"),
                             Command.Create("subCmd2",
                                 options: new [] {Option.Create<int?>("subCmd2-opt2", "help me!") })
                      }),
                 Command.Create("cmd2", "", ArgumentList.Create(arity: Arity.ZeroOrOne, "arg2")),
                 Command.Create("cmd3")
            });

        private readonly Command _command2 =
            CommandLine.Create("",
               commands: new[] {
                  Command.Create("the-command", "help!",
                     argument: ArgumentList.Create("arg1")
                        .WithArity(Arity.OneOrMore),
                     options: new[] {
                         Option.Create("opt1"),
                         Option.Create<int?>("opt2")
                            .WithHelp("help me!"),
                         Option.Create<int?>( "opt3"),
                         Option.Create<int>("opt4")
                            .WithArgumentList(  "opt_arg1",Arity.ExactlyOne,defaultValue: 0),
                         Option.Create<string>("opt5")
                            .WithArgumentList( "opt_arg2", arity: Arity.ZeroOrMore)
                     },
                     commands: new[] {Command.Create("subCmd1"),
                         Command.Create("subCmd2")
                            .WithOptions( Option.Create<int?>("subCmd2-opt2", "help me!"))
                     }),
                  Command.Create<string>("cmd2")
                     .WithArgumentList("arg2", arity: Arity.ZeroOrOne),
                  Command.Create("cmd3")
            });

        private readonly Command _command =
            CommandLine.Create("","",
               Command.Create("the-command", "help!",
                    ArgumentList.Create(arity: Arity.OneOrMore, "arg1"),
                    Option.Create("opt1"),
                    Option.Create<int?>("opt2", "help me!"),
                    Option.Create<int?>(arity: Arity.ExactlyOne, "opt3", ""),
                    Option.Create("opt4", "", argument: ArgumentList.Create(arity: Arity.ExactlyOne, "opt_arg1", defaultValue: 0)),
                    Option.Create("opt5", "", argument: ArgumentList.Create<int?>(arity: Arity.ZeroOrMore, "opt_arg2")),
                    Command.Create("subCmd1"),
                    Command.Create("subCmd2", "", Option.Create<int?>("subCmd2-opt2", "help me!"))
                    ),
               Command.Create("cmd2", "", ArgumentList.Create(arity: Arity.ZeroOrOne, "arg2")),
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
            var visitor = new ReportVisitor();
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
