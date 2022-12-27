using System;

namespace CSF
{
    /// <summary>
    ///     Represents certain flags about a parameter.
    /// </summary>
    [Flags]
    public enum ParameterFlags : byte
    {
        /// <summary>
        ///     Defines a default value.
        /// </summary>
        Default = 1,

        /// <summary>
        ///     Defines if a parameter is nullable.
        /// </summary>
        IsNullable = 2,

        /// <summary>
        ///     Defines if a parameter is optional.
        /// </summary>
        IsOptional = 4,

        /// <summary>
        ///     Defines if a parameter is remainder.
        /// </summary>
        IsRemainder = 8,
    }

    internal static class ParameterFlagsExtensions
    {
        public static ParameterFlags WithNullable(this ParameterFlags flags, bool isNullable = true)
        {
            if (isNullable)
                flags |= ParameterFlags.IsNullable;
            return flags;
        }

        public static ParameterFlags WithOptional(this ParameterFlags flags, bool isOptional = true)
        {
            if (isOptional)
                flags |= ParameterFlags.IsOptional;
            return flags;
        }

        public static ParameterFlags WithRemainder(this ParameterFlags flags, bool isRemainder = true)
        {
            if (isRemainder)
                flags |= ParameterFlags.IsRemainder;
            return flags;
        }
    }
}
