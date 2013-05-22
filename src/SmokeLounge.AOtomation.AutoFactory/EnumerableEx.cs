// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableEx.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the EnumerableEx type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public static class EnumerableEx
    {
        #region Public Methods and Operators

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null);
            Contract.Requires<ArgumentNullException>(action != null);
            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        #endregion
    }
}