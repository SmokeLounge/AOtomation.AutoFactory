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
    using System.Linq.Expressions;
    using System.Reflection;

    [Export(typeof(IAutoFactory<>))]
    public class MefAutoFactory<T> : IAutoFactory<T>
        where T : class
    {
        #region Fields

        private readonly CompositionContainer compositionContainer;

        private readonly Lazy<Func<object[], T>> lazyFactoryDelegate;

        private readonly Lazy<CtorInfo> lazyTypeCtor;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MefAutoFactory(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
            this.lazyTypeCtor = new Lazy<CtorInfo>(() => this.FindCtor(typeof(T)));
            this.lazyFactoryDelegate = new Lazy<Func<object[], T>>(() => this.CreateFactoryDelegate<T>(this.TypeCtor));
        }

        #endregion

        #region Public Properties

        public Func<object[], T> FactoryDelegate
        {
            get
            {
                return this.lazyFactoryDelegate.Value;
            }
        }

        public CtorInfo TypeCtor
        {
            get
            {
                return this.lazyTypeCtor.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public T Create(params object[] args)
        {
            return this.FactoryDelegate(this.EnsureArgs(this.TypeCtor.ParamTypes, args).ToArray());
        }

        #endregion

        #region Methods

        private Func<object[], TK> CreateFactoryDelegate<TK>(CtorInfo ctor)
        {
            var arrayExpression = Expression.Parameter(typeof(object[]));
            var paramExpressions = new List<Expression>();
            var ctorParameters = ctor.ParamTypes;
            for (var i = 0; i < ctorParameters.Length; i++)
            {
                var indexExpression = Expression.ArrayIndex(arrayExpression, Expression.Constant(i));
                var unboxExpression = Expression.Convert(indexExpression, ctorParameters[i]);
                paramExpressions.Add(unboxExpression);
            }

            var newExpression = Expression.New(ctor.ConstructorInfo, paramExpressions);
            var newLambda = Expression.Lambda<Func<object[], TK>>(newExpression, arrayExpression);
            var compiledLambda = newLambda.Compile();
            return compiledLambda;
        }

        private IEnumerable<object> EnsureArgs(IEnumerable<Type> ctorParams, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return ctorParams.Select(GetOrCreateNewParam);
            }

            return args.Concat(ctorParams.Skip(args.Length).Select(GetOrCreateNewParam));
        }

        private CtorInfo FindCtor(Type type)
        {
            var ctor = (from constructorInfo in type.GetConstructors()
                        where constructorInfo.GetCustomAttribute<ImportingConstructorAttribute>() != null
                        let parameters = constructorInfo.GetParameters()
                        orderby parameters.Count() descending
                        select new CtorInfo(constructorInfo)).FirstOrDefault();
            if (ctor == null)
            {
                throw new Exception("Could not find a suitable constructor.");
            }

            return ctor;
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

        private object GetOrCreateNewParam(Type type)
        {
            if (!type.IsInterface || !type.IsGenericType)
            {
                return this.GetExportedValue(type);
            }

            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(IAutoFactory<>))
            {
                var mefAutoFactory = typeof(MefAutoFactory<>).MakeGenericType(type.GetGenericArguments());
                var import = mefAutoFactory.GetInstanceDynamic(this.GetExportedValue(typeof(CompositionContainer)));
                return import;
            }

            return this.GetExportedValue(type);
        }

        #endregion
    }
}