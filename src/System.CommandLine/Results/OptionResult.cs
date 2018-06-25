namespace System.CommandLine.Result
{
    public class OptionResult : BaseResult
    {
        public string SpecifiedToken { get; internal set; }
        public bool IsImplicit { get; internal set; }
        public bool IsSet { get; internal set; }
    }
}
