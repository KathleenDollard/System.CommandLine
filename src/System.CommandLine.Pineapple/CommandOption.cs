namespace System.CommandLine.Pineapple
{
    public class CommandOption
    {
        private readonly OptionDefinition _definition;

        public CommandOption(OptionDefinition definition)
        {
            _definition = definition;
        }

        public string Value()
        {
            throw new NotImplementedException();
            ;
        }
    }
}
