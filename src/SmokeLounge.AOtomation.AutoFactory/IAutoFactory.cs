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
        #region Public Properties

        Func<object[], T> FactoryDelegate { get; }

        IMiniIoC MiniIoC { get; }

        CtorInfo TypeCtor { get; }

        #endregion

        #region Public Methods and Operators

        T Create(params object[] args);

        #endregion
    }

    [ContractClassFor(typeof(IAutoFactory<>))]
    internal abstract class IAutoFactoryContract<T> : IAutoFactory<T>
        where T : class
    {
        #region Public Properties

        public Func<object[], T> FactoryDelegate
        {
            get
            {
                Contract.Ensures(Contract.Result<Func<object[], T>>() != null);

                throw new NotImplementedException();
            }
        }

        public IMiniIoC MiniIoC
        {
            get
            {
                Contract.Ensures(Contract.Result<IMiniIoC>() != null);

                throw new NotImplementedException();
            }
        }

        public CtorInfo TypeCtor
        {
            get
            {
                Contract.Ensures(Contract.Result<CtorInfo>() != null);

                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods and Operators

        public T Create(params object[] args)
        {
            Contract.Ensures(Contract.Result<T>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }
}