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
                return this.constructorInfo;
            }
        }

        public Type[] ParamTypes
        {
            get
            {
                return this.paramTypes;
            }
        }

        public ParameterInfo[] Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        #endregion
    }
}