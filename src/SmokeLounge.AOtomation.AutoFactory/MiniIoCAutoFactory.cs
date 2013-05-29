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
        private readonly IAutoFactory<T> parent;

        #region Fields

        #endregion

        #region Constructors and Destructors

        public MiniIoCAutoFactory(IAutoFactory<T> parent, IMiniIoC miniIoC)
            : base(miniIoC)
        {
            Contract.Requires<ArgumentNullException>(parent != null);
            Contract.Requires<ArgumentNullException>(miniIoC != null);

            this.parent = parent;
        }

        #endregion

        #region Public Properties

        public override Func<object[], T> FactoryDelegate
        {
            get
            {
                return this.parent.FactoryDelegate;
            }
        }

        public override CtorInfo TypeCtor
        {
            get
            {
                return this.parent.TypeCtor;
            }
        }

        protected internal override object GetOrCreateNewParam(Type type)
        {
            var param = base.GetOrCreateNewParam(type);
            if (param != null)
            {
                return param;
            }

            var factoryImp = this.parent as AbstractAutoFactory<T>;
            return factoryImp != null ? factoryImp.GetOrCreateNewParam(type) : null;
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.parent != null);
        }

        #endregion
    }
}