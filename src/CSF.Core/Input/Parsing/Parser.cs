using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     The default parameter parser for provided command phrases.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        ///     Parses the provided raw input into a list of parameters, including the command name.
        /// </summary>
        /// <remarks>
        ///     This parser will <see langword="not"/> handle any steps in removing the prefix of the input. This needs to be done prior to running the parser.
        /// </remarks>
        /// <param name="rawInput">The input to parse.</param>
        /// <returns>A new <see cref="ParserOutput"/> from the input string.</returns>
        public static ParserOutput Parse(string rawInput)
        {
            var range = rawInput.Split(' ');

            var name = "";
            var param = new List<string>();
            var partials = new List<string>();

            var flagname = "";
            var flags = new Dictionary<string, object>();

            foreach (var entry in range)
            {
                if (name is "")
                {
                    name = entry;
                    continue;
                }

                if (partials.Any())
                {
                    if (entry.EndsWith("\""))
                    {
                        partials.Add(entry.Replace("\"", ""));

                        if (flagname is "")
                            param.Add(string.Join(" ", partials));
                        else
                        {
                            flags.Add(flagname, string.Join(" ", partials));
                            flagname = "";
                        }

                        partials.Clear();
                        continue;
                    }
                    partials.Add(entry);
                    continue;
                }

                if (entry.StartsWith("\""))
                {
                    if (entry.EndsWith("\""))
                    {
                        if (flagname is "")
                            param.Add(entry.Replace("\"", ""));
                        else
                        {
                            flags.Add(flagname, entry.Replace("\"", ""));
                            flagname = "";
                        }
                    }
                    else
                        partials.Add(entry.Replace("\"", ""));
                    continue;
                }

                if (entry.StartsWith("-"))
                {
                    if (!entry.EndsWith(":"))
                        flags.Add(entry.Substring(1), null);
                    else
                        flagname = entry.Substring(1, entry.Length - 2);
                    continue;
                }

                param.Add(entry);
            }

            return new ParserOutput(name, param, flags);
        }
    }
}
