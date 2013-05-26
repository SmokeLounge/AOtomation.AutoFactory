// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefIoC.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the MefIoC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Linq;

    [Export]
    public class MefIoC : IMiniIoC
    {
        #region Fields

        private readonly CompositionContainer compositionContainer;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MefIoC(CompositionContainer compositionContainer)
        {
            Contract.Requires<ArgumentNullException>(compositionContainer != null);

            this.compositionContainer = compositionContainer;
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
            return this.compositionContainer.GetExportedValueOrDefault<T>();
        }

        public object GetInstance(Type type)
        {
            return
                this.compositionContainer.GetExportedValues<object>(AttributedModelServices.GetContractName(type))
                    .FirstOrDefault();
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.compositionContainer != null);
        }

        #endregion
    }
}