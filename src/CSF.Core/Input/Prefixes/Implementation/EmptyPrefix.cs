using System.Collections.Generic;

namespace CSF
{
    public readonly struct EmptyPrefix : IPrefix<string>
    {
        /// <inheritdoc/>
        public string Value { get; }

        /// <summary>
        ///     Creates a new <see cref="StringPrefix"/> with provided string.
        /// </summary>
        /// <param name="prefix"></param>
        private EmptyPrefix(string prefix)
            => Value = prefix;

        /// <inheritdoc/>
        public string GetValueAs()
        {
            return Value;
        }

        /// <inheritdoc/>
        public bool Equals(IPrefix prefix)
        {
            if (prefix.Value == Value)
                return true;
            return false;
        }

        /// <summary>
        ///     Calls the inner <see cref="Equals(IPrefix)"/> member after verifying the incoming value matches <see cref="StringPrefix"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is StringPrefix prefix && Equals(prefix))
                return true;
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 290341713 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        /// <summary>
        ///     Creates a new empty prefix. Thanks to .netstandard2.0 not allowing empty struct constructors, it is not possible to do this by simply calling new.
        /// </summary>
        /// <returns></returns>
        public static EmptyPrefix Create()
        {
            return new EmptyPrefix("");
        }
    }
}
