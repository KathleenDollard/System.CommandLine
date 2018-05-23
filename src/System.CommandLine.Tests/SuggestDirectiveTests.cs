// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static System.Environment;

namespace System.CommandLine.Tests
{
    public class SuggestDirectiveTests
    {
        private readonly ITestOutputHelper output;

        public SuggestDirectiveTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Parse_directive_writes_parse_diagram()
        {
            var parser = new ParserBuilder()
                         .AddCommand("eat", "",
                                     cmd => cmd.AddOption(new[] { "--fruit" }, "",
                                                          args => args.AddSuggestions("apple", "banana", "cherry")))
                         .UseSuggestDirective()
                         .Build();

            var result = parser.Parse("!suggest eat --fruit ");
            
            var console = new TestConsole();

            await result.InvokeAsync(console);

            console.Out
                   .ToString()
                   .Should()
                   .Be($"apple{NewLine}banana{NewLine}cherry{NewLine}");
        }
    }
}
