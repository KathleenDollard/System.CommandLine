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

            app.AddOption<string>("-n", "--first-name", "The name of the person to greet");
            app.AddOption<string>("--last-name", "The last name of the person");

            return app.Run(DoSomething, args);
        }

        private static int DoSomething(Option name, Option lastName)
        {
            Console.WriteLine($"Hello {name.GetValueOrDefault()} {lastName.GetValueOrDefault()}");
            return 0;
        }
    }
}
