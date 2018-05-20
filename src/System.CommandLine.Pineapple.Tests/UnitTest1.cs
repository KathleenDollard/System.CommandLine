using System;
using Xunit;

namespace System.CommandLine.Pineapple.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var app = new CommandLineApp();

            // PineappleSample.exe hello -n "Inigo Montoya"
            var defaultCmd = app.AddCommand("hello", (Option name) => {
                Console.WriteLine($"Hello {name.GetValueOrDefault<string>()}");
                return 0;
            });
            defaultCmd.AddOption("-n", "--name", "The name of the person to greet");

            // PineappleSample.exe call --phoneNumber "800-FourFun"
            var cmd = app.AddCommand("call", (Option phoneNumber) => {
                Console.WriteLine($"Starting a phone call to .... {phoneNumber.GetValueOrDefault()}");
                return 0;
            });
            cmd.AddOption("-p", "--phoneNumber", "Phone number");


            var parser = app.GetParser();

            ParseResult result = parser.Parse("testhost hello -n Yuriy");

            Assert.Empty(result.Errors);
        }
    }
}
