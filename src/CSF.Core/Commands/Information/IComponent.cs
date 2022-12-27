using System;
using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents any part of a command.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        ///     Represents the name of the component in question.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The attribute collection for this component.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }
    }
}
