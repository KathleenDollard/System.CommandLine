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

            app.AddOption<string>("-n", "--name", "The name of the person to greet");

            return app.Run(DoSomething, args);
        }

        private static int DoSomething(Option name)
        {
            Console.WriteLine("Hello " + name.GetValueOrDefault());
            return 0;
        }
    }
}
