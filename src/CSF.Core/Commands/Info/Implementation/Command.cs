using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class Command : IConditionalComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public Module Module { get; }

        /// <summary>
        ///     The list of parameters for this command.
        /// </summary>
        public IReadOnlyCollection<Parameter> Parameters { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Represents if the command is an error overload.
        /// </summary>
        public bool IsErrorOverload { get; }

        internal Command(CommandConfiguration config, Module module, MethodInfo method, string[] aliases)
        {
            Method = method;
            Module = module;

            Attributes = module.Attributes.Concat(GetAttributes()).ToList();
            Preconditions = module.Preconditions.Concat(GetPreconditions()).ToList();

            Parameters = GetParameters(config).ToList();

            var remainderParameters = Parameters.Where(x => x.Flags.HasFlag(ParameterFlags.IsRemainder));
            if (remainderParameters.Any())
            {
                if (remainderParameters.Count() > 1)
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} cannot exist on multiple parameters at once.");

                if (!Parameters.Last().Flags.HasFlag(ParameterFlags.IsRemainder))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
            }

            if (Attributes.Any(x => x is ErrorOverloadAttribute))
            {
                if (Parameters.Any())
                    throw new InvalidOperationException($"{nameof(ErrorOverloadAttribute)} cannot exist on a method with parameters.");

                IsErrorOverload = true;
            }

            Name = aliases[0];
            Aliases = aliases;
        }

        private IEnumerable<Parameter> GetParameters(CommandConfiguration config)
        {
            foreach (var param in Method.GetParameters())
                yield return new Parameter(param, config.TypeReaders);
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attribute in Method.GetCustomAttributes(true))
                if (attribute is Attribute attr)
                    yield return attr;
        }
    }
}
