using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents an injectable service.
    /// </summary>
    public sealed class DependencyInfo : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        internal DependencyInfo(System.Reflection.ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;

            Attributes = GetAttributes(paramInfo).ToList();
            Flags = SetFlags(paramInfo);

            Name = paramInfo.Name;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DependencyInfo Build(System.Reflection.ParameterInfo paramInfo)
        {
            return new DependencyInfo(paramInfo);
        }

        private IEnumerable<Attribute> GetAttributes(System.Reflection.ParameterInfo paramInfo)
        {
            foreach (var obj in paramInfo.GetCustomAttributes(false))
                if (obj is Attribute attribute)
                    yield return attribute;
        }

        private ParameterFlags SetFlags(System.Reflection.ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var flags = ParameterFlags.Default
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            return flags;
        }
    }
}
