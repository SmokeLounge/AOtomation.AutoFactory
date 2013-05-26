// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoFactoryExtensions.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the AutoFactoryExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Diagnostics.Contracts;

    public static class AutoFactoryExtensions
    {
        #region Public Methods and Operators

        public static IAutoFactory<T> WithIoC<T>(this IAutoFactory<T> source, IMiniIoC miniIoC) where T : class
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(miniIoC != null);
            Contract.Ensures(Contract.Result<IAutoFactory<T>>() != null);

            return new MiniIoCAutoFactory<T>(
                source.TypeCtor, source.FactoryDelegate, new CompositeIoC(new[] { miniIoC, source.MiniIoC }));
        }

        #endregion
    }
}