﻿using System.Reflection;
using Proxier.Interfaces;

namespace Proxier.Contexts
{
    internal class CopyContext : ICopyContext
    {
        /// <inheritdoc />
        public PropertyInfo Property { get; }

        /// <inheritdoc />
        public object Source { get; }

        /// <inheritdoc />
        public object Target { get; }

        public CopyContext(PropertyInfo property, object source, object target)
        {
            Property = property;
            Source = source;
            Target = target;
        }
    }
}