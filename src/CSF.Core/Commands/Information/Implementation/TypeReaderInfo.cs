using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public sealed class TypeReaderInfo : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The type of this reader.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The constructor used to create an instance of the reader type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        internal TypeReaderInfo(Type type)
        {
            Type = type;
            Name = type.Name;
            Constructor = new ConstructorInfo(type);
            Attributes = GetAttributes()
                .ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TypeReaderInfo Build(Type type)
        {
            if (type == typeof(ITypeReader))
            {
                if (type.ContainsGenericParameters)
                    throw new InvalidOperationException("Untyped generic types cannot be automatically registered. Consider defining DontRegisterAttribute and manually adding them.");
            }
            else
                throw new InvalidOperationException($"Provided type is not a valid {nameof(ITypeReader)}.");

            return new TypeReaderInfo(type);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ITypeReader Construct(IServiceProvider provider)
        {
            var obj = Constructor.Construct(provider);

            if (obj is ITypeReader reader)
                return reader;
            else
                return null;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }
    }
}
