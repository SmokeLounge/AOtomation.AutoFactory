// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMiniIoC.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the IMiniIoC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(IMiniIoCContract))]
    public interface IMiniIoC
    {
        #region Public Methods and Operators

        void AddInstance<T>(T instance) where T : class;

        void AddInstance(Type type, object instance);

        T GetInstance<T>() where T : class;

        object GetInstance(Type type);

        #endregion
    }

    [ContractClassFor(typeof(IMiniIoC))]
    internal abstract class IMiniIoCContract : IMiniIoC
    {
        #region Public Methods and Operators

        public void AddInstance<T>(T instance) where T : class
        {
            Contract.Requires<ArgumentNullException>(instance != null);

            throw new NotImplementedException();
        }

        public void AddInstance(Type type, object instance)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            Contract.Requires<ArgumentNullException>(instance != null);
            Contract.Requires<ArgumentNullException>(type.IsInstanceOfType(instance));

            throw new NotImplementedException();
        }

        public T GetInstance<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            throw new NotImplementedException();
        }

        #endregion
    }
}