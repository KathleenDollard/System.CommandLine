// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine.Parser
{
    public class FailedArgumentTypeConversionResult : FailedArgumentParseResult
    {
        // TODO: (FailedArgumentTypeConversionResult) localize
        public FailedArgumentTypeConversionResult(Type type, string value) : 
            base($"Cannot parse argument '{value}' as {type}.")
        {
        }
    }
}
