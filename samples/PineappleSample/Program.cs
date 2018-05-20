using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Pineapple;
using System.Security.Cryptography;

namespace PineappleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApp();

            // PineappleSample.exe hello -n "Inigo Montoya"
            var defaultCmd = app.AddCommand("hello", SayHello);
            defaultCmd.AddOption( "-n", "--name", "The name of the person to greet");

            // PineappleSample.exe call --phoneNumber "800-FourFun"
            var cmd = app.AddCommand("call", CallPhoneNumber);
            cmd.AddOption( "-p", "--phoneNumber", "Phone number");

            while(true)
            {
                Console.WriteLine("Ready for input...");
                app.Run(Console.ReadLine());
            }

        }

        private static int SayHello(Option name)
        {
            Console.WriteLine($"Hello {name.GetValueOrDefault<string>()}");
            return 0;
        }

        private static int CallPhoneNumber(Option phoneNumber)
        {
            Console.WriteLine($"Starting a phone call to .... {phoneNumber.GetValueOrDefault()}");
            return 0;
        }
    }
}
