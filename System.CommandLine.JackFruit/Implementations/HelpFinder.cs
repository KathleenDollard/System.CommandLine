﻿using System.Reflection;

namespace System.CommandLine.JackFruit
{

    public class HelpFinder : FinderBase<HelpFinder, string>
    {
        protected static (bool, string) FromAttribute(Command parent, object source)
        {
            switch (source)
            {
                // This will need more work when source is a tuple with type/name, etc
                case Type type:
                    return GetHelp(type.GetCustomAttribute<HelpAttribute>(), type.Name);
                case PropertyInfo propertyInfo:
                    return GetHelp(propertyInfo.GetCustomAttribute<HelpAttribute>(), propertyInfo.Name);
                case ParameterInfo parameterInfo:
                    return GetHelp(parameterInfo.GetCustomAttribute<HelpAttribute>(), parameterInfo.Name);
                default:
                    return (false, null);
            }

            (bool, string) GetHelp(HelpAttribute attribute, string name)
                => attribute != null
                    ? (false, attribute.HelpText)
                    : (false, null);
        }

        protected static (bool, string) FromDescription
            (IDescriptionFinder descriptionProvider, Command parent, object source, object item)
        {
            var ret = (descriptionProvider != null && (source.Equals(item) || item == null)
                   ? descriptionProvider?.Description(source)
                   : descriptionProvider?.Description(source, NameFromItem()))
                      ?? null;
            return (false, ret);

            string NameFromItem()
            {
                switch (item)
                {
                    case PropertyInfo p:
                        return p.Name;
                    case ParameterInfo p:
                        return p.Name;
                    default:
                        return null;
                }
            }
        }

        public static HelpFinder Default() 
            => new HelpFinder()
                .AddApproachFromFunc<object>(FromAttribute);

       
        public HelpFinder AddDescriptionFinder(IDescriptionFinder descriptionFinder)
        {
            AddApproachFromFunc<object>(
                   (parent, source) => FromDescription(descriptionFinder, parent, source, source));
            return this;
        }
    }
}