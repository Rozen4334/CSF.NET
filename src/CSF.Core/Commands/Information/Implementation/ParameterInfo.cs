using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a single parameter for the method.
    /// </summary>
    public sealed class ParameterInfo : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The typereader used to populate this parameter.
        /// </summary>
        public ITypeReader TypeReader { get; }

        internal ParameterInfo(System.Reflection.ParameterInfo paramInfo, TypeReaderProvider typeReaders)
        {
            var type = paramInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;
            TypeReader = GetTypeReader(typeReaders);

            Attributes = GetAttributes(paramInfo).ToList();
            Flags = SetFlags(paramInfo);

            Name = paramInfo.Name;
        }

        private ITypeReader GetTypeReader(TypeReaderProvider typeReaders)
        {
            if (typeReaders.TryGetReader(Type, out var reader))
                return reader;

            throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {Type.Name}.");
        }

        private IEnumerable<Attribute> GetAttributes(System.Reflection.ParameterInfo paramInfo)
        {
            foreach (var attribute in paramInfo.GetCustomAttributes(false))
                if (attribute is Attribute attr)
                    yield return attr;
        }

        private ParameterFlags SetFlags(System.Reflection.ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var flags = ParameterFlags.Default
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            if (Attributes.Any(x => x is RemainderAttribute))
            {
                if (Type != typeof(string))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on string parameters.");

                flags = flags.WithRemainder();
            }

            return flags;
        }
    }
}
