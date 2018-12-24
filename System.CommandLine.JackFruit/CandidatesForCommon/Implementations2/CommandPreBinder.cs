﻿using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.CommandLine.JackFruit
{
    public class CommandPreBinder
    {
        public static Command ParserTreeFromDerivedTypes<TRootType>(IDescriptionFinder descriptionFinder)
        {
            PreBinderContext.Current.SubCommandFinder.AddApproach(
                CommandFinder.DerivedTypeApproach(typeof(TRootType)));
            PreBinderContext.Current.HelpFinder.AddApproach(
                HelpFinder.DescriptionFinderApproach(descriptionFinder));
            var command = CommandFinder.GetCommand(typeof(TRootType), new RootCommand());
            return command;
        }
    }
}
