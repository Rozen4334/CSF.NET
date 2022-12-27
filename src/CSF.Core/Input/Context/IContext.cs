using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents an interface that supports all implementations of command context classes.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The command parameters.
        /// </summary>
        IReadOnlyList<object> Parameters { get; set; }
    }
}
