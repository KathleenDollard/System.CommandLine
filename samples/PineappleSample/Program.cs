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

            app.AddOption("-n", "--name", "The name of the person to greet");

            app.OnExecute(cmd =>
            {
                Console.WriteLine("Hello " + cmd["name"].GetValueOrDefault());
            });

            return app.Run(args);
        }
    }
}
