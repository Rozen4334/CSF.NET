using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents information about the primary constructor of a module.
    /// </summary>
    public class ConstructorInfo : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The parameters of this module.
        /// </summary>
        public IReadOnlyCollection<DependencyInfo> Dependencies { get; }

        /// <summary>
        ///     The constructor entry point.
        /// </summary>
        public System.Reflection.ConstructorInfo EntryPoint { get; }

        internal ConstructorInfo(Type type)
        {
            EntryPoint = GetEntryConstructor(type);
            Attributes = GetAttributes(EntryPoint).ToList();

            Dependencies = GetDependencies(EntryPoint).ToList();

            Name = EntryPoint.Name;
        }

        public object Construct(IServiceProvider provider)
        {
            var services = new List<object>();
            foreach (var dependency in Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return null;

                    services.Add(t);
                }
            }

            var obj = EntryPoint.Invoke(services.ToArray());

            return obj;
        }

        private System.Reflection.ConstructorInfo GetEntryConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            if (!constructors.Any())
                throw new InvalidOperationException($"Found no constructor on provided module type: {type.Name}");

            var constructor = constructors[0];

            if (constructors.Length is 1)
                return constructor;

            for (int i = 0; i < constructors.Length; i++)
                foreach (var attribute in constructors[i].GetCustomAttributes(true))
                    if (attribute is InjectionConstructorAttribute)
                        return constructors[i];

            return constructor;
        }

        private IEnumerable<Attribute> GetAttributes(System.Reflection.ConstructorInfo ctorInfo)
        {
            foreach (var obj in ctorInfo.GetCustomAttributes(false))
                if (obj is Attribute attribute)
                    yield return attribute;
        }

        private IEnumerable<DependencyInfo> GetDependencies(System.Reflection.ConstructorInfo ctorInfo)
        {
            foreach (var param in ctorInfo.GetParameters())
                yield return new DependencyInfo(param);
        }
    }
}