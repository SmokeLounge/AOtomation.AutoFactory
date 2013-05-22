// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeTree.cs" company="SmokeLounge">
//   Copyright © 2013 SmokeLounge.
//   This program is free software. It comes without any warranty, to
//   the extent permitted by applicable law. You can redistribute it
//   and/or modify it under the terms of the Do What The Fuck You Want
//   To Public License, Version 2, as published by Sam Hocevar. See
//   http://www.wtfpl.net/ for more details.
// </copyright>
// <summary>
//   Defines the InvokeTree type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmokeLounge.AOtomation.AutoFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal class InvokeTree
    {
        #region Fields

        private readonly InvokeTreeNode root;

        #endregion

        #region Constructors and Destructors

        public InvokeTree()
        {
            this.root = new InvokeTreeNode();
        }

        #endregion

        #region Public Methods and Operators

        public void AddMethod(Type[] args, Func<Type, object[], object> method)
        {
            Contract.Requires<ArgumentNullException>(args != null);
            var invokeTree = this.root;
            foreach (var type in args)
            {
                var child = invokeTree.GetChild(type);
                invokeTree = child ?? invokeTree.AddChild(type);
            }

            invokeTree.Func = method;
        }

        public Func<Type, object[], object> GetMethod(Type[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null);
            var invokeTree = this.root;
            foreach (var type in args)
            {
                invokeTree = invokeTree.GetChild(type);
                if (invokeTree == null)
                {
                    return null;
                }
            }

            return invokeTree.Func;
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.root != null);
        }

        #endregion

        private class InvokeTreeNode
        {
            #region Fields

            private readonly Dictionary<Type, InvokeTreeNode> children;

            #endregion

            #region Constructors and Destructors

            public InvokeTreeNode()
            {
                this.children = new Dictionary<Type, InvokeTreeNode>();
            }

            #endregion

            #region Public Properties

            public Func<Type, object[], object> Func { get; set; }

            #endregion

            #region Public Methods and Operators

            public InvokeTreeNode AddChild(Type type)
            {
                var child = new InvokeTreeNode();
                this.children.Add(type, child);
                return child;
            }

            public InvokeTreeNode GetChild(Type type)
            {
                InvokeTreeNode node;
                return this.children.TryGetValue(type, out node) ? node : null;
            }

            #endregion

            #region Methods

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.children != null);
            }

            #endregion
        }
    }
}