﻿namespace System.CommandLine.JackFruit
{
    public interface IDescriptionFinder
    {
        string Description<TSource>(TSource source);
        string Description<TSource, TItem>(TSource source, TItem child);
    }
}
