namespace System.CommandLine.API
{
    public static class PartExtensions
    {

        public static TPart WithHelp<TPart>(this TPart part, string help)
            where TPart : BasePart
        {
            part.Help = help;
            return part;
        }
    }


}
