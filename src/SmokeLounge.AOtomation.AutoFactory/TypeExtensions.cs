// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the TypeExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class TypeExtensions
    {
        #region Static Fields

        private static readonly InvokeTree invokeTree = new InvokeTree();

        #endregion

        #region Public Methods and Operators

        public static object GetInstance(this Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return Activator.CreateInstance(type);
        }

        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return GetInstance<TArg, TypeToIgnore>(type, argument, null);
        }

        public static object GetInstance<TArg1, TArg2>(this Type type, TArg1 argument1, TArg2 argument2)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return GetInstance<TArg1, TArg2, TypeToIgnore>(type, argument1, argument2, null);
        }

        public static object GetInstance<TArg1, TArg2, TArg3>(
            this Type type, TArg1 argument1, TArg2 argument2, TArg3 argument3)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            return InstanceCreationFactory<TArg1, TArg2, TArg3>.CreateInstanceOf(type, argument1, argument2, argument3);
        }

        public static object GetInstanceDynamic(this Type type, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            if (args == null || args.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            var genericTypeArguments = args.Select(a => a is Type ? typeof(Type) : a.GetType()).ToArray();

            var func = invokeTree.GetMethod(genericTypeArguments);

            if (func == null)
            {
                func = CreateFunc(genericTypeArguments);
                invokeTree.AddMethod(genericTypeArguments, func);
            }

            return func(type, args);
        }

        #endregion

        #region Methods

        private static Func<Type, object[], object> CreateFunc(Type[] genericTypeArguments)
        {
            Contract.Requires<ArgumentNullException>(genericTypeArguments != null);
            Contract.Ensures(Contract.Result<Func<Type, object[], object>>() != null);

            var genericParameterCount = genericTypeArguments.Length + 1;
            var methodInfos = typeof(TypeExtensions).GetMethods();
            Contract.Assume(methodInfos.Count() > 0);
            var methodInfo =
                methodInfos.First(m => m.Name == "GetInstance" && m.GetParameters().Length == genericParameterCount);

            Contract.Assume(methodInfo != null);
            var genericMethod = methodInfo.MakeGenericMethod(genericTypeArguments);

            var param0 = Expression.Parameter(typeof(Type));
            var param1 = Expression.Parameter(typeof(object[]));

            var varExps = genericTypeArguments.Select(Expression.Variable).ToArray();
            var callElementAtExps =
                varExps.Select(
                    (v, i) =>
                    Expression.Convert(Expression.ArrayIndex(param1, Expression.Constant(i)), genericTypeArguments[i]));
            var callExp = Expression.Call(genericMethod, new Expression[] { param0 }.Concat(callElementAtExps));

            Contract.Assume(callExp != null);
            var lambda = Expression.Lambda<Func<Type, object[], object>>(callExp, param0, param1);
            var func = lambda.Compile();

            Contract.Assume(func != null);
            return func;
        }

        #endregion

        private static class InstanceCreationFactory<TArg1, TArg2, TArg3>
        {
            #region Static Fields

            private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, object>> instanceCreationMethods =
                new Dictionary<Type, Func<TArg1, TArg2, TArg3, object>>();

            #endregion

            #region Public Methods and Operators

            public static object CreateInstanceOf(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            {
                Contract.Requires<ArgumentNullException>(type != null);
                Func<TArg1, TArg2, TArg3, object> constructor;
                if (instanceCreationMethods.TryGetValue(type, out constructor) == false)
                {
                    constructor = CreateConstructor(type);
                    instanceCreationMethods[type] = constructor;
                }

                Contract.Assume(constructor != null);
                return constructor.Invoke(arg1, arg2, arg3);
            }

            #endregion

            #region Methods

            private static Func<TArg1, TArg2, TArg3, object> CreateConstructor(Type type)
            {
                Contract.Requires<ArgumentNullException>(type != null);
                Contract.Ensures(Contract.Result<Func<TArg1, TArg2, TArg3, object>>() != null);
                var argumentTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

                var constructorArgumentTypes = argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();

                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public, 
                    null, 
                    CallingConventions.HasThis, 
                    constructorArgumentTypes, 
                    new ParameterModifier[0]);

                if (constructor == null)
                {
                    throw new ApplicationException("Constructor could not be found.");
                }

                var lamdaParameterExpressions = new[]
                                                    {
                                                        Expression.Parameter(typeof(TArg1)), 
                                                        Expression.Parameter(typeof(TArg2)), 
                                                        Expression.Parameter(typeof(TArg3))
                                                    };

                var constructorParameterExpressions =
                    lamdaParameterExpressions.Take(constructorArgumentTypes.Length).Cast<Expression>().ToArray();

                var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);

                var constructorCallingLambda =
                    Expression.Lambda<Func<TArg1, TArg2, TArg3, object>>(
                        constructorCallExpression, lamdaParameterExpressions).Compile();

                Contract.Assume(constructorCallingLambda != null);
                return constructorCallingLambda;
            }

            #endregion
        }

        private class TypeToIgnore
        {
        }
    }
}