﻿using System.Collections;
using GN.CodeGuard.Internals;

namespace GN.CodeGuard
{
    public static class CollectionValidatorExtensions
    {
        public static ArgBase<T> IsNotEmpty<T>(this ArgBase<T> arg)
            where T: class, ICollection
        {
            arg.IsNotNull();

            if (arg.Value.Count == 0)
                arg.ThrowArgument("Collection is empty");

            return arg;
        }
    }
}