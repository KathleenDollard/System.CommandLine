using System;
using Xunit;
using System.CommandLine.DotNetBanana;

namespace System.CommandLine.DotNetBanana.Tests
{
    public class SimpleSampleTests
    {

        public class ApplicationCommandLine

        {
            public string Name { get; set; }
            public string Age { get; set; }
        }

        [Fact]
        public void Instantiate_ApplicationCommandLine_WithNameSet()
        {
            string[] args = { "Inigo Montoya" };
            ApplicationCommandLine commandLine = CommandLineParser.Run<ApplicationCommandLine>(args);
            Assert.Equal(args[0], commandLine.Name);
        }
    }
}
