// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeIoC.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the CompositeIoC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class CompositeIoC : IMiniIoC
    {
        #region Fields

        private readonly IEnumerable<IMiniIoC> miniIoCs;

        #endregion

        #region Constructors and Destructors

        public CompositeIoC(IEnumerable<IMiniIoC> miniIoCs)
        {
            Contract.Requires<ArgumentNullException>(miniIoCs != null);

            this.miniIoCs = miniIoCs;
        }

        #endregion

        #region Public Methods and Operators

        public void AddInstance<T>(T instance) where T : class
        {
            throw new InvalidOperationException();
        }

        public void AddInstance(Type type, object instance)
        {
            throw new InvalidOperationException();
        }

        public T GetInstance<T>() where T : class
        {
            return this.GetInstance(typeof(T)) as T;
        }

        public object GetInstance(Type type)
        {
            return
                this.miniIoCs.Select(miniIoC => new { miniIoC, instance = miniIoC.GetInstance(type) })
                    .Where(@t => @t.instance != null)
                    .Select(@t => @t.instance)
                    .FirstOrDefault();
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.miniIoCs != null);
        }

        #endregion
    }
}