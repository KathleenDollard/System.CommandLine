// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.CommandLine
{
    public interface IConsole
    {
        TextWriter Out { get; }

        TextWriter Error { get; }

        ConsoleColor ForegroundColor { get; set; }

        int WindowWidth { get; }

        void ResetColor();
    }
}