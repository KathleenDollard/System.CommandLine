﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.CommandLine.JackFruit
{
    public class CommandFinder : FinderBase<IEnumerable<Command>>
    {
        public CommandFinder(params Approach<IEnumerable<Command>>[] approaches)
            : base(approaches: approaches)
        { }

        private class DerivedTypeFinder
        {
            private IEnumerable<IGrouping<Type, Type>> typesByBase;

            internal DerivedTypeFinder(Type rootType)
            {
                typesByBase = rootType.Assembly
                                  .GetTypes()
                                  .GroupBy(x => x.BaseType);
            }

            internal IEnumerable<Type> GetDerivedTypes(Type baseType)
            => typesByBase
                        .Where(x => x.Key == baseType)
                        .SingleOrDefault();
        }

        private static (bool, IEnumerable<Command>) FromDerivedTypes(
                 DerivedTypeFinder derivedTypeFinder,object parent, Type baseType)
        {
            var derivedTypes = derivedTypeFinder.GetDerivedTypes(baseType)
                                    ?.Select(t => GetCommand(parent, t))
                                    .ToList();
            return (derivedTypes == null || derivedTypes.Any(), derivedTypes);
        }

        // TODO: Filter this for Ignore methods
        private static (bool, IEnumerable<Command>) FromMethod(object parent, Type baseType)
        {
            var methods = baseType.GetMethods(Reflection.Constants.PublicThisInstance)
                            .Where(m => !m.IsSpecialName);
            var commands = methods
                            .Select(m => GetCommand(parent, m))
                            .ToList();
            return ((commands != null && commands.Any(), commands));

            //var method = baseType.GetMethod("InvokeAsync", Reflection.Constants.PublicAndInstance);
            //return (method != null, PreBinderContext.Current.SubCommandFinder.Get(method)) ;
        }

        // Command is passed in for Root command
        internal static Command GetCommand<T>(object parent, T source, Command command = null)
        {
            // Arguments vs. Options - Fix has to handle args defined in parent type for hybrid: 
            // Approach - create both and remove the option after creation - extra work, but no order dependency
            // Alternate - add the options to the command earlier and pass to OptionFinder
            var names = PreBinderContext.Current.AliasFinder.Get(parent, source);
            var help = PreBinderContext.Current.HelpFinder.Get(parent, source);
            var arguments = PreBinderContext.Current.ArgumentFinder.Get(parent, source);
            var options = PreBinderContext.Current.OptionFinder.Get(parent, source);
            var handler = PreBinderContext.Current.HandlerFinder.Get(parent, source);
            command = command ?? new Command(names?.First(), help);
            if (arguments.Any())
            {
                // TODO: When multi-arguments merged, update this
                command.Argument = arguments.First();
            }
            var subCommands = PreBinderContext.Current.SubCommandFinder.Get(parent, source);
            command.Handler = handler;
            command.AddOptions(options);
            return command;
        }

        public static Approach<IEnumerable<Command>> DerivedTypeApproach(Type rootType)
            => Approach<IEnumerable<Command>>.CreateApproach<Type>(
                          (p,t) => FromDerivedTypes(new DerivedTypeFinder(rootType), p,t));

        public static Approach<IEnumerable<Command>> MethodApproach()
            => Approach<IEnumerable<Command>>.CreateApproach<Type>(FromMethod);

        public static CommandFinder Default()
            => new CommandFinder(MethodApproach());
    }
}
