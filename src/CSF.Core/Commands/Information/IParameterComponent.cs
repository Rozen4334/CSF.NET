using System;

namespace CSF
{
    /// <summary>
    ///     Represents a constructor or method parameter.
    /// </summary>
    public interface IParameterComponent : IComponent
    {
        /// <summary>
        ///     The type of the parameter.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     The flag collection specifying member data about the parameter.
        /// </summary>
        ParameterFlags Flags { get; }
    }
}
