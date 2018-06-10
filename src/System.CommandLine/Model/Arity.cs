// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.CommandLine
{
    // This version allows IEnumerable or not to be determined with
    // "arity Is Arity.Single"  or "arity is Arity.Many"
    // and restriction in parameters as "Arity.Single arity"
    public class Arity
    {
        public static readonly Single Zero = new Single();
        public static readonly Single ZeroOrOne = new Single();
        public static readonly Single ExactlyOne = new Single();
        public static readonly Many ZeroOrMore = new Many();
        public static readonly Many OneOrMore = new Many();
        public static readonly Arity Default = ZeroOrMore;

        public class Many : Arity
        {
            // This is burned is a default, so once released, do not change!
            public static new readonly Many Default = ZeroOrMore;
        }
        public class Single : Arity
        {
            // This is burned is a default, so once released, do not change!
            public static new readonly Single Default = ZeroOrOne;
        }
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
