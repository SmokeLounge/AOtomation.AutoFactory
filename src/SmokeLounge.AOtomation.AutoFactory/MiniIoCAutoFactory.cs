// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MiniIoCAutoFactory.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the MiniIoCAutoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Diagnostics.Contracts;

    public class MiniIoCAutoFactory<T> : AbstractAutoFactory<T>
        where T : class
    {
        #region Fields

        private readonly CtorInfo ctorInfo;

        private readonly Func<object[], T> factoryDelegate;

        #endregion

        #region Constructors and Destructors

        public MiniIoCAutoFactory(CtorInfo ctorInfo, Func<object[], T> factoryDelegate, IMiniIoC miniIoC)
            : base(miniIoC)
        {
            Contract.Requires<ArgumentNullException>(ctorInfo != null);
            Contract.Requires<ArgumentNullException>(factoryDelegate != null);
            Contract.Requires<ArgumentNullException>(miniIoC != null);

            this.ctorInfo = ctorInfo;
            this.factoryDelegate = factoryDelegate;
        }

        #endregion

        #region Public Properties

        public override Func<object[], T> FactoryDelegate
        {
            get
            {
                return this.factoryDelegate;
            }
        }

        public override CtorInfo TypeCtor
        {
            get
            {
                return this.ctorInfo;
            }
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.ctorInfo != null);
            Contract.Invariant(this.factoryDelegate != null);
        }

        #endregion
    }
}