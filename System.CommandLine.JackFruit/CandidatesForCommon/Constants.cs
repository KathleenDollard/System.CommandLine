﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.CommandLine.JackFruit.Reflection
{
    public class Constants
    {
        public static readonly BindingFlags PublicThisInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    }
}