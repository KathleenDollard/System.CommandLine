﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Rendering;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace System.CommandLine.DragonFruit.Tests
{
    public class CommandLineTests
    {
        private readonly TestTerminal _terminal;
        private readonly TestProgram _testProgram;

        public CommandLineTests()
        {
            _terminal = new TestTerminal();
            _testProgram = new TestProgram();
        }

        [Fact]
        public async Task It_executes_method_with_string_option()
        {
            int exitCode = await CommandLine.InvokeMethodAsync(
                               new[] { "--name", "Wayne" },
                               _terminal,
                               TestProgram.TestMainMethodInfo,
                               _testProgram);
            exitCode.Should().Be(0);
            _testProgram.Captured.Should().Be("Wayne");
        }

        [Fact]
        public async Task It_shows_help_text_based_on_XML_documentation_comments()
        {
            int exitCode = await CommandLine.InvokeMethodAsync(
                               new[] { "--help" },
                               _terminal,
                               TestProgram.TestMainMethodInfo,
                               _testProgram);

            exitCode.Should().Be(0);

            var stdOut = _terminal.Out.ToString();

            stdOut.Should()
                  .Contain("--name       Specifies the name option")
                  .And.Contain("Options:");
            stdOut.Should()
                  .Contain("Help for the test program");
        }

        [Fact]
        public async Task It_executes_method_with_string_option_with_default()
        {
            int exitCode = await CommandLine.InvokeMethodAsync(
                               Array.Empty<string>(),
                               _terminal,
                               TestProgram.TestMainMethodInfoWithDefault,
                               _testProgram);

            exitCode.Should().Be(0);
            _testProgram.Captured.Should().Be("Bruce");
        }

        private void TestMainThatThrows() => throw new InvalidOperationException("This threw an error");

        [Fact]
        public async Task It_shows_error_without_invoking_method()
        {
            Action action = TestMainThatThrows;

            int exitCode = await CommandLine.InvokeMethodAsync(
                               new[] { "--unknown" },
                               _terminal,
                               action.Method,
                               this);

            exitCode.Should().Be(1);
            _terminal.Error.ToString()
                    .Should().NotBeEmpty()
                    .And
                    .Contain("--unknown");
            _terminal.ForegroundColor.Should().Be(ConsoleColor.Red);
        }

        [Fact]
        public async Task It_handles_uncaught_exceptions()
        {
            Action action = TestMainThatThrows;

            int exitCode = await CommandLine.InvokeMethodAsync(
                               Array.Empty<string>(),
                               _terminal,
                               action.Method,
                               this);

            exitCode.Should().Be(1);
            _terminal.Error.ToString()
                    .Should().NotBeEmpty()
                    .And
                    .Contain("This threw an error");
            _terminal.ForegroundColor.Should().Be(ConsoleColor.Red);
        }
    }
}
