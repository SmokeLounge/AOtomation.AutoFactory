// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractAutoFactory.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the AbstractAutoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public abstract class AbstractAutoFactory<T> : IAutoFactory<T>
        where T : class
    {
        #region Fields

        private readonly IMiniIoC miniIoC;

        #endregion

        #region Constructors and Destructors

        protected AbstractAutoFactory(IMiniIoC miniIoC)
        {
            Contract.Requires(miniIoC != null);

            this.miniIoC = miniIoC;
        }

        #endregion

        #region Public Properties

        public abstract Func<object[], T> FactoryDelegate { get; }

        public IMiniIoC MiniIoC
        {
            get
            {
                return this.miniIoC;
            }
        }

        public abstract CtorInfo TypeCtor { get; }

        #endregion

        #region Public Methods and Operators

        public virtual T Create(params object[] args)
        {
            var obj = this.FactoryDelegate(this.EnsureArgs(this.TypeCtor.ParamTypes, args).ToArray());
            if (obj == null)
            {
                throw new InvalidOperationException();
            }

            return obj;
        }

        #endregion

        #region Methods

        protected IEnumerable<object> EnsureArgs(IEnumerable<Type> ctorParams, params object[] args)
        {
            Contract.Requires(ctorParams != null);

            if (args == null || args.Length == 0)
            {
                return ctorParams.Select(this.GetOrCreateNewParam);
            }

            return args.Concat(ctorParams.Skip(args.Length).Select(this.GetOrCreateNewParam));
        }

        protected virtual object GetOrCreateNewParam(Type type)
        {
            Contract.Requires(type != null);

            return this.MiniIoC.GetInstance(type);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.miniIoC != null);
        }

        #endregion
    }
}