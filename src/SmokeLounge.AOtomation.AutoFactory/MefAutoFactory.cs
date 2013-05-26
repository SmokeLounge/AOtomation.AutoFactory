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
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    [Export(typeof(IAutoFactory<>))]
    public class MefAutoFactory<T> : AbstractAutoFactory<T>
        where T : class
    {
        #region Fields

        private readonly Lazy<Func<object[], T>> lazyFactoryDelegate;

        private readonly Lazy<CtorInfo> lazyTypeCtor;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MefAutoFactory(MefIoC mefIoC)
            : base(mefIoC)
        {
            Contract.Requires(mefIoC != null);

            this.lazyTypeCtor = new Lazy<CtorInfo>(() => this.FindCtor(typeof(T)));
            this.lazyFactoryDelegate = new Lazy<Func<object[], T>>(() => this.CreateFactoryDelegate<T>(this.TypeCtor));
        }

        #endregion

        #region Public Properties

        public override Func<object[], T> FactoryDelegate
        {
            get
            {
                var value = this.lazyFactoryDelegate.Value;
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                return value;
            }
        }

        public override CtorInfo TypeCtor
        {
            get
            {
                var value = this.lazyTypeCtor.Value;
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                return value;
            }
        }

        #endregion

        #region Methods

        protected override object GetOrCreateNewParam(Type type)
        {
            if (!type.IsInterface || !type.IsGenericType)
            {
                return this.MiniIoC.GetInstance(type);
            }

            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(IAutoFactory<>))
            {
                var mefAutoFactory = typeof(MefAutoFactory<>).MakeGenericType(type.GetGenericArguments());
                var import = mefAutoFactory.GetInstanceDynamic(this.MiniIoC);
                return import;
            }

            return this.MiniIoC.GetInstance(type);
        }

        private Func<object[], TK> CreateFactoryDelegate<TK>(CtorInfo ctor)
        {
            Contract.Requires(ctor != null);

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

        private CtorInfo FindCtor(Type type)
        {
            Contract.Requires(type != null);

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.lazyFactoryDelegate != null);
            Contract.Invariant(this.lazyTypeCtor != null);
        }

        #endregion
    }
}