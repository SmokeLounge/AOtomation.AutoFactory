// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MiniIoC.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the MiniIoC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.Contracts;

    public class MiniIoC : IMiniIoC
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, object> instances;

        #endregion

        #region Constructors and Destructors

        public MiniIoC()
        {
            this.instances = new ConcurrentDictionary<Type, object>();
        }

        #endregion

        #region Public Methods and Operators

        public void AddInstance<T>(T instance) where T : class
        {
            this.instances.AddOrUpdate(typeof(T), instance, (t, o) => instance);
        }

        public void AddInstance(Type type, object instance)
        {
            this.instances.AddOrUpdate(type, instance, (t, o) => instance);
        }

        public T GetInstance<T>() where T : class
        {
            return this.GetInstance(typeof(T)) as T;
        }

        public object GetInstance(Type type)
        {
            object instance;
            this.instances.TryGetValue(type, out instance);
            return instance;
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.instances != null);
        }

        #endregion
    }
}