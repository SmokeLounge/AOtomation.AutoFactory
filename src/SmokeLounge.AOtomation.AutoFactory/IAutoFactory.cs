// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutoFactory.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the IAutoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IAutoFactoryContract<>))]
    public interface IAutoFactory<out T>
        where T : class
    {
        #region Public Methods and Operators

        T Create(params object[] args);

        #endregion
    }

    [ContractClassFor(typeof(IAutoFactory<>))]
    internal abstract class IAutoFactoryContract<T> : IAutoFactory<T>
        where T : class
    {
        #region Public Methods and Operators

        public T Create(params object[] args)
        {
            Contract.Ensures(Contract.Result<T>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }
}