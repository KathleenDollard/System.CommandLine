using System;
using System.CommandLine;
using System.CommandLine.Pineapple;
using System.Security.Cryptography;

namespace PineappleSample
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApp();

            var defaultCmd = app.AddCommand("", SayHello);
            defaultCmd.AddOption<string>("--lastName", "The last name of the person");
            defaultCmd.AddOption<string>("-n", "--firstName", "The name of the person to greet");

            var cmd1 = app.AddCommand("call business", CallPhoneNumber);
            var cmd2 = app.AddCommand("call person", CallPhoneNumber);
            cmd.AddOption<string>("-p", "--phone-number", "Phone number");

            return app.Run(args);
        }

        private static int SayHello(Option firstName, Option lastName)
        {
            Console.WriteLine($"Hello {firstName.GetValueOrDefault<string>()} {lastName.GetValueOrDefault()}");
            return 0;
        }

        private static int CallPhoneNumber(Option phoneNumber)
        {
            Console.WriteLine($"Starting a phone call to .... {phoneNumber.GetValueOrDefault()}");
            return 0;
        }
    }
}
