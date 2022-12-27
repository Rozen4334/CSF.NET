using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Defines the output of a parsed raw text command input.
    /// </summary>
    public readonly struct ParserOutput
    {
        /// <summary>
        ///     The name of the parsed input.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The flags of the input. <see langword="null"/> if flag parsing is disabled.
        /// </summary>
        public IReadOnlyDictionary<string, object> Flags { get; }

        /// <summary>
        ///     The parameters of the input.
        /// </summary>
        public IReadOnlyList<object> Parameters { get; }

        /// <summary>
        ///     Creates a new <see cref="ParserOutput"/> from provided name and param.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public ParserOutput(string name, List<string> parameters)
            : this(name, parameters, null)
        {

        }

        /// <summary>
        ///     Creates a new <see cref="ParserOutput"/> from provided variables.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="flags"></param>
        public ParserOutput(string name, List<string> parameters, Dictionary<string, object> flags)
        {
            Name = name;
            Parameters = parameters;
            Flags = flags;
        }
    }
}
