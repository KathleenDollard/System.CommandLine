// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Reflection;

namespace System.CommandLine.Builder
{
    public static class ParserBuilderExtensions
    {
        public static TBuilder AddCommand<TBuilder>(
            this TBuilder builder,
            string name,
            string description = null,
            Action<CommandDefinitionBuilder> symbols = null,
            Action<ArgumentDefinitionBuilder> arguments = null)
            where TBuilder : CommandDefinitionBuilder
        {
            var commandDefinitionBuilder = new CommandDefinitionBuilder(name, builder) {
                Description = description
            };

            symbols?.Invoke(commandDefinitionBuilder);

            arguments?.Invoke(commandDefinitionBuilder.Arguments);

            builder.Commands.Add(commandDefinitionBuilder);

            return builder;
        }

        public static TBuilder ConfigureFromMethod<TBuilder>(
            this TBuilder builder,
            MethodInfo method,
            object target = null)
            where TBuilder : CommandDefinitionBuilder
        {
            foreach (var parameter in method.GetParameters())
            {
                builder.AddOptionFromParameter(parameter);
            }

            builder.OnExecute(method, target);

            return builder;
        }

        public static TBuilder AddOptionFromParameter<TBuilder>(
            this TBuilder builder,
            ParameterInfo parameter)
            where TBuilder : CommandDefinitionBuilder
        {
            string paramName = parameter.Name.ToKebabCase();

            builder.AddOption(
                new[] {
                    "-" + paramName[0],
                    "--" + paramName,
                },
                parameter.Name,
                args => {
                    args.ParseArgumentsAs(parameter.ParameterType);

                    if (parameter.HasDefaultValue)
                    {
                        args.WithDefaultValue(() => parameter.DefaultValue);
                    }
                });

            return builder;
        }

        public static TBuilder AddOption<TBuilder>(
            this TBuilder builder,
            string[] aliases,
            string description = null,
            Action<ArgumentDefinitionBuilder> arguments = null)
            where TBuilder : CommandDefinitionBuilder
        {
            var optionDefinitionBuilder = new OptionDefinitionBuilder(aliases, builder) {
                Description = description
            };

            arguments?.Invoke(optionDefinitionBuilder.Arguments);

            builder.Options.Add(optionDefinitionBuilder);

            return builder;
        }

        public static TBuilder AddOption<TBuilder>(
            this TBuilder builder,
            string name,
            string description = null,
            Action<ArgumentDefinitionBuilder> arguments = null)
            where TBuilder : CommandDefinitionBuilder
        {
            return builder.AddOption(new[] { name }, description, arguments);
        }

        public static ParserBuilder EnablePosixBundling(
            this ParserBuilder builder,
            bool value = true)
        {
            builder.EnablePosixBundling = value;
            return builder;
        }

        public static ParserBuilder TreatUnmatchedTokensAsErrors(
            this ParserBuilder builder,
            bool value = true)
        {
            builder.TreatUnmatchedTokensAsErrors = value;
            return builder;
        }

        public static TBuilder AddArguments<TBuilder>(
            this TBuilder builder,
            Action<ArgumentDefinitionBuilder> action)
            where TBuilder : CommandDefinitionBuilder
        {
            action.Invoke(builder.Arguments);
            return builder;
        }

        public static ParserBuilder ParseResponseFileAs(
            this ParserBuilder builder,
            ResponseFileHandling responseFileHandling)
        {
            builder.ResponseFileHandling = responseFileHandling;
            return builder;
        }

        public static TBuilder UsePrefixes<TBuilder>(this TBuilder builder, IReadOnlyCollection<string> prefixes)
            where TBuilder : ParserBuilder
        {
            builder.Prefixes = prefixes;
            return builder;
        }
    }
}
