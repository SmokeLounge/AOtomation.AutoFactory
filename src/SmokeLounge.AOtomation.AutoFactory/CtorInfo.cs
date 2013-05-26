// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtorInfo.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the CtorInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    public class CtorInfo
    {
        #region Fields

        private readonly ConstructorInfo constructorInfo;

        private readonly Type[] paramTypes;

        private readonly ParameterInfo[] parameters;

        #endregion

        #region Constructors and Destructors

        public CtorInfo(ConstructorInfo constructorInfo)
        {
            Contract.Requires<ArgumentNullException>(constructorInfo != null);

            this.constructorInfo = constructorInfo;
            this.parameters = constructorInfo.GetParameters();
            this.paramTypes = constructorInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        #endregion

        #region Public Properties

        public ConstructorInfo ConstructorInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<ConstructorInfo>() != null);

                return this.constructorInfo;
            }
        }

        public Type[] ParamTypes
        {
            get
            {
                Contract.Ensures(Contract.Result<Type[]>() != null);

                return this.paramTypes;
            }
        }

        public ParameterInfo[] Parameters
        {
            get
            {
                Contract.Ensures(Contract.Result<ParameterInfo[]>() != null);

                return this.parameters;
            }
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.constructorInfo != null);
            Contract.Invariant(this.paramTypes != null);
            Contract.Invariant(this.parameters != null);
        }

        #endregion
    }
}