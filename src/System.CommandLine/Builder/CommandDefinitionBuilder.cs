// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;

namespace System.CommandLine.Builder
{
    public class CommandDefinitionBuilder : SymbolDefinitionBuilder
    {
        public CommandDefinitionBuilder(
            string name,
            CommandDefinitionBuilder parent = null) : base(parent)
        {
            Name = name;
        }

        public OptionDefinitionBuilderSet Options { get; } = new OptionDefinitionBuilderSet();

        public CommandDefinitionBuilderSet Commands { get; } = new CommandDefinitionBuilderSet();

        public bool? TreatUnmatchedTokensAsErrors { get; set; }

        internal MethodBinder ExecutionHandler { get; set; }

        public string Name { get; }

        public CommandDefinition BuildCommandDefinition()
        {
            return new CommandDefinition(
                Name,
                Description,
                argumentDefinition: BuildArguments(),
                symbolDefinitions: BuildChildSymbolDefinitions(),
                treatUnmatchedTokensAsErrors: TreatUnmatchedTokensAsErrors ??
                                              Parent?.TreatUnmatchedTokensAsErrors ??
                                              true,
                executionHandler: ExecutionHandler);
        }

        protected IReadOnlyCollection<SymbolDefinition> BuildChildSymbolDefinitions()
        {
            var subcommands = Commands
                .Select(b => {
                    b.TreatUnmatchedTokensAsErrors = TreatUnmatchedTokensAsErrors;
                    return b.BuildCommandDefinition();
                });

            var options = Options
                .Select(b => b.BuildOptionDefinition());

            return subcommands.Concat<SymbolDefinition>(options).ToArray();
        }
    }


}
