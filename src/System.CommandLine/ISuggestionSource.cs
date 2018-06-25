// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.CommandLine.Parser
{
    public interface ISuggestionSource
    {
        IEnumerable<string> Suggest(
            ParseResult parseResult,
            int? position = null);
    }
}
