using System;
using System.CommandLine;
using System.CommandLine.Pineapple;

namespace PineappleSample
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApp();

            app.AddOption<string>("-n", "--firstName", "The name of the person to greet");
            app.AddOption<string>("--lastName", "The last name of the person");

            return app.Run(SayHello, args);
        }

        private static int SayHello(Option firstName, Option lastName)
        {
            Console.WriteLine($"Hello {firstName.GetValueOrDefault()} {lastName.GetValueOrDefault()}");
            return 0;
        }
    }
}
