// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefAutoFactory.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the MefAutoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    [Export(typeof(IAutoFactory<>))]
    public class MefAutoFactory<T> : IAutoFactory<T>
        where T : class
    {
        #region Fields

        private readonly CompositionContainer compositionContainer;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MefAutoFactory(CompositionContainer compositionContainer)
        {
            Contract.Requires<ArgumentNullException>(compositionContainer != null);

            this.compositionContainer = compositionContainer;
        }

        #endregion

        #region Public Methods and Operators

        public T Create(params object[] args)
        {
            // TODO: Optimize this method. use Castle.DynamicProxy?
            args = args ?? new object[0];
            var ctor = this.FindConstructor(args);
            if (ctor == null)
            {
                if (args.Any() == false)
                {
                    return (T)Activator.CreateInstance(typeof(T));
                }

                throw new Exception("Could not find a suitable constructor.");
            }

            var imports = this.FindImports(ctor, args);

            var ctorParams = new object[args.Length + imports.Length];
            Array.Copy(args, 0, ctorParams, 0, args.Length);
            Array.Copy(imports, 0, ctorParams, args.Length, imports.Length);

            var export = typeof(T).GetInstanceDynamic(ctorParams);
            return (T)export;
        }

        #endregion

        #region Methods

        private ConstructorInfo FindConstructor(object[] args)
        {
            Contract.Requires(args != null);

            var ctors = new List<ConstructorInfo>();
            ctors.AddRange(
                typeof(T).GetConstructors().Where(c => c.GetCustomAttribute<ImportingConstructorAttribute>() != null));

            foreach (var ctor in ctors)
            {
                var ctorParameters = ctor.GetParameters();

                if (args.Length > ctorParameters.Length)
                {
                    continue;
                }

                var match = true;
                for (var i = 0; i < args.Length; i++)
                {
                    var argType = args[i].GetType();
                    var paramType = ctorParameters[i].ParameterType;
                    if (paramType.IsAssignableFrom(argType))
                    {
                        continue;
                    }

                    match = false;
                    break;
                }

                if (match)
                {
                    return ctor;
                }
            }

            return null;
        }

        private object[] FindImports(ConstructorInfo ctor, object[] args)
        {
            Contract.Requires(ctor != null);
            Contract.Requires(args != null);

            var ctorParameters = ctor.GetParameters().Skip(args.Length);
            var imports = new List<object>();
            foreach (var ctorParameter in ctorParameters)
            {
                var import = this.GetOrCreateImport(ctorParameter.ParameterType);

                imports.Add(import);
            }

            return imports.ToArray();
        }

        private object GetExportedValue(Type type)
        {
            var contract = AttributedModelServices.GetContractName(type);
            var import = this.compositionContainer.GetExportedValueOrDefault<object>(contract);
            if (import == null)
            {
                throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
            }

            return import;
        }

        private object GetOrCreateImport(Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<object>() != null);

            if (type.IsInterface && type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(IAutoFactory<>))
                {
                    var mefAutoFactory = typeof(MefAutoFactory<>).MakeGenericType(type.GetGenericArguments());
                    var import = mefAutoFactory.GetInstanceDynamic(this.GetExportedValue(typeof(CompositionContainer)));
                    return import;
                }
            }

            return this.GetExportedValue(type);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.compositionContainer != null);
        }

        #endregion
    }
}