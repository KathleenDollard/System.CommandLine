using System;
using System.Collections.Generic;
using System.Text;

namespace System.CommandLine
{
    public interface ISymbol
    {
        //string Token { get; }
    }

    public interface IHasArgument : ISymbol // happens to always have a symbol. This might be an assumption
    {
        Argument Argument { get; }
        //Func<object> DefaultValueFunc { get; }
        //object DefaultValue { get; }
        //Arity Arity { get; }
    }
}
