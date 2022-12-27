using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public sealed class ModuleInfo : IConditionalComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     The components of this module.
        /// </summary>
        public IReadOnlyCollection<IConditionalComponent> Components { get; }

        /// <summary>
        ///     The type of this module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The constructor used to create an instance of the command type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        ///     The root module. <see langword="null"/> if not available.
        /// </summary>
        public ModuleInfo Root { get; }

        internal ModuleInfo(TypeReaderProvider typeReaders, Type type, ModuleInfo rootModule = null, string expectedName = null, string[] aliases = null)
        {
            if (rootModule != null)
                Root = rootModule;

            Type = type;
            Constructor = new ConstructorInfo(type);

            Attributes = (Root?.Attributes.Concat(GetAttributes()) ?? GetAttributes()).ToList();
            Preconditions = (Root?.Preconditions.Concat(GetPreconditions()) ?? GetPreconditions()).ToList();

            Name = expectedName ?? type.Name;
            Aliases = aliases ?? new string[] { Name };

            Components = GetComponents(typeReaders).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ModuleInfo Build(TypeReaderProvider typeReaders, Type type)
        {
            var module = new ModuleInfo(typeReaders, type);

            return module;
        }

        private IEnumerable<IConditionalComponent> GetComponents(TypeReaderProvider typeReaders)
        {
            foreach (var method in Type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(true);

                string[] aliases = Array.Empty<string>();
                foreach (var attribute in attributes)
                {
                    if (attribute is CommandAttribute commandAttribute)
                    {
                        aliases = commandAttribute.Aliases;
                    }
                }

                if (!aliases.Any())
                    continue;

                yield return new CommandInfo(typeReaders, this, method, aliases);
            }

            foreach (var group in Type.GetNestedTypes())
            {
                foreach (var attribute in group.GetCustomAttributes(true))
                {
                    if (attribute is GroupAttribute gattribute)
                        yield return new ModuleInfo(typeReaders, group, Root, gattribute.Name, gattribute.Aliases);
                }
            }
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }
    }
}
