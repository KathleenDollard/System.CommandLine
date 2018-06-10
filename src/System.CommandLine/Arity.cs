// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine
{
    // This version allows IEnumerable or not to be determined with
    // "arity Is Arity.Single"  or "arity is Arity.Many"
    // and restriction in parameters as "Arity.Single arity"
    public class Arity
    {
        public static Single Zero = new Single();
        public static Single ZeroOrOne = new Single();
        public static Single ExactlyOne = new Single();
        public static Many ZeroOrMore = new Many();
        public static Many OneOrMore = new Many();

        public class Many : Arity { }
        public class Single : Arity { }
    }


    //public enum Arity
    //{
    //    NotSpecified = 0,
    //    Zero,
    //    ZeroOrOne,
    //    ExactlyOne,
    //    ZeroOrMore,
    //    OneOrMore

    //}
}
