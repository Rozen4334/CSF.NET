using System.Collections.Generic;

namespace CSF
{
    /// <inheritdoc/>
    public readonly struct CharPrefix : IPrefix<char>
    {
        /// <inheritdoc/>
        public string Value { get; }

        /// <summary>
        ///     Creates a new <see cref="CharPrefix"/> with provided character.
        /// </summary>
        /// <param name="prefix"></param>
        public CharPrefix(char prefix)
            => Value = $"{prefix}";

        /// <inheritdoc/>
        public char GetValueAs()
        {
            return Value.ToCharArray()[0];
        }

        /// <inheritdoc/>
        public bool Equals(IPrefix prefix)
        {
            if (prefix.Value == Value)
                return true;
            return false;
        }

        /// <summary>
        ///     Calls the inner <see cref="Equals(IPrefix)"/> member after verifying the incoming value matches <see cref="CharPrefix"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is CharPrefix prefix && Equals(prefix))
                return true;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 290341713 + EqualityComparer<string>.Default.GetHashCode(Value);
        }
    }
}
