using System;
using System.CommandLine;



// TODO: Should this be a value type? Probably.
public class CommandLineParameter<T>
{
    protected const bool DefaultIsRequired = false;
    static private string DefaultHelpDescription = $"A string that can successfully convert to an { typeof(T).FullName }";
    public string HelpDescription { get; } =
        DefaultHelpDescription;

    public bool IsRequired { get; }

    public string Name { get; }

    internal CommandLineParameter() : this(DefaultHelpDescription)
    {

    }

    public CommandLineParameter(string helpDescription, bool isRequired = DefaultIsRequired) :
        this(null, helpDescription, isRequired)
    {
    }

    public CommandLineParameter(string name, string helpDescription, bool isRequired = DefaultIsRequired)
    {
        HelpDescription = helpDescription;
        IsRequired = isRequired;
        Name = name;
    }
}

public static class CommandLineParameter
{
    static public CommandLineParameter<bool> CreateSwitch(string name, string description, bool isRequired) =>
        new CommandLineParameter<bool>(name, description, isRequired);
    static public CommandLineParameter<T> CreateOption<T>(string name, string description, bool isRequired) =>
        new CommandLineParameter<T>(name, description, isRequired);
}


public class SampleFile
{
    public static void SampleMain(string[] args)
    {
        // TODO: Can we, should we support a description for the program?
        // (This would be the description for the base (process) command.
        TryParseAs(
            args, out (string, string, bool) result1,
            new CommandLineParameter<string>("Description", true)
            , new CommandLineParameter<string>("FirstName", "Description")
            , new CommandLineParameter<bool>("IsDryRun", "Description")
            );

        // TODO: Provide examples for a list of arguments of the same type. e.g. names
        // TODO: Handle arguments where order doesn't matter

    }
    public static (int first, int second) SampleMain((string first, string second) param)
    {
        throw new Exception();
    }

    public static (int first, int second) Main(string param1 = "ignore")
    {
        throw new Exception();
    }

    public static bool TryParseAs<T1, T2, T3>(string[] args,
        out (T1, T2, T3) result,
        CommandLineParameter<T1> parameter1 = default(CommandLineParameter<T1>),
        CommandLineParameter<T2> parameter2 = default(CommandLineParameter<T2>),
        CommandLineParameter<T3> parameter3 = default(CommandLineParameter<T3>)

        )
    {
        if (parameter1 == default(CommandLineParameter<T1>))
        {
            parameter1 = new CommandLineParameter<T1>();
        }
        if (parameter2 == default(CommandLineParameter<T2>))
        {
            parameter2 = new CommandLineParameter<T2>();
        }
        if (parameter3 == default(CommandLineParameter<T3>))
        {
            parameter3 = new CommandLineParameter<T3>();
        }

        ArgumentRuleBuilder argumentRuleBuilder = new ArgumentRuleBuilder();

        if(parameter1.Name is null)
        {
            if (parameter1.IsRequired)
            {

            }
        }

        Command command = Create.Command(System.Diagnostics.Process.GetCurrentProcess().ProcessName, "");

    //    Create.Option("-x", "", ArgumentsRule.None),
    //Create.Option("-y", "", ArgumentsRule.None),
    //Create.Option("-z", "", ArgumentsRule.None),
    //Create.Option("-xyz", "", ArgumentsRule.None));
    //    var parseConfig = new ParserConfiguration(new[] { command }, allowUnbundling: false);
    //    var parser = new CommandParser(parseConfig);
    //    var result = parser.Parse("the-command -xyz");

    //    result
    //        .ParsedCommand()
    //        .Children
    //        .Select(o => o.Name)
    //        .Should()
    //        .BeEquivalentTo("xyz");


        throw new NotImplementedException();
    }
}

