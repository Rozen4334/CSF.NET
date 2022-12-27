using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a provider for prefixes, which can be used to define <see cref="RequirePrefixAttribute"/> and to define limitations in <see cref="CommandFramework.TryParsePrefix(ref string, out IPrefix)"/>
    /// </summary>
    public sealed class PrefixProvider
    {
        private readonly Dictionary<string, IPrefix> _prefixes;

        /// <summary>
        ///     Creates a new typereader dictionary with all default readers.
        /// </summary>
        public PrefixProvider()
            : this(new Dictionary<string, IPrefix>())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderProvider"/> with self-defined default readers.
        /// </summary>
        /// <param name="dictionary"></param>
        public PrefixProvider(Dictionary<string, IPrefix> dictionary)
        {
            _prefixes = dictionary;
        }

        /// <summary>
        ///     Gets or sets a typereader for the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IPrefix this[string key]
        {
            get
                => _prefixes[key];
            set
                => _prefixes[key] = value;
        }

        /// <summary>
        ///     Includes an <see cref="IPrefix"/> in the <see cref="PrefixProvider"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public PrefixProvider Include<T>(IPrefix<T> prefix)
            => Include((IPrefix)prefix);

        /// <summary>
        ///     Includes an <see cref="IPrefix"/> in the <see cref="PrefixProvider"/>.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public PrefixProvider Include(IPrefix prefix)
        {
            _prefixes.Add(prefix.Value, prefix);
            return this;
        }

        /// <summary>
        ///     Excludes an <see cref="IPrefix"/> from the <see cref="PrefixProvider"/>.
        /// </summary>
        /// <param name="prefix">The value for which to remove the <see cref="IPrefix"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public PrefixProvider Exclude<T>(IPrefix<T> prefix)
            => Exclude(prefix);

        /// <summary>
        ///     Excludes an <see cref="IPrefix"/> from the <see cref="PrefixProvider"/>.
        /// </summary>
        /// <param name="prefix">The value for which to remove the <see cref="IPrefix"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public PrefixProvider Exclude(IPrefix prefix)
            => Exclude(prefix.Value);

        /// <summary>
        ///     Excludes an <see cref="IPrefix"/> from the <see cref="PrefixProvider"/>.
        /// </summary>
        /// <param name="prefix">The value for which to remove the <see cref="IPrefix"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public PrefixProvider Exclude(string prefix)
        {
            _prefixes.Remove(prefix);
            return this;
        }

        /// <summary>
        ///     Tries to get a <see cref="IPrefix"/> from the underlying dictionary.
        /// </summary>
        /// <param name="rawInput">The raw command input to fetch a prefix for.</param>
        /// <param name="prefix"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetPrefix(string rawInput, out IPrefix prefix)
        {
            prefix = null;

            foreach (var value in _prefixes)
            {
                if (rawInput.StartsWith(value.Value.Value))
                {
                    prefix = value.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Tries to get a <see cref="IPrefix"/> from the underlying dictionary.
        /// </summary>
        /// <remarks>
        ///     This method is intended for precise values. Use <see cref="TryGetPrefix(string, out IPrefix)"/> to fetch a prefix for command phrases.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawInput"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public bool TryGetPrefix<T>(string rawInput, out IPrefix<T> prefix)
        {
            prefix = null;
            if (_prefixes.TryGetValue(rawInput, out var value) && value is IPrefix<T> returnValue)
            {
                prefix = returnValue;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///     Gets if the <see cref="PrefixProvider"/> has any <see cref="IPrefix"/>'s defined.
        /// </summary>
        /// <returns><see langword="true"/> if there are any <see cref="IPrefix"/>'s inside the current <see cref="PrefixProvider"/>. <see langword="false"/> if not.</returns>
        public bool HasValues()
            => _prefixes.Any();

        /// <summary>
        ///     Copies all keys in the current dictionary to another, overwriting existing keys.
        /// </summary>
        /// <param name="targetDictionary">The target dictionary to copy to.</param>
        public void CopyTo(PrefixProvider targetDictionary)
        {
            foreach (var kvp in _prefixes)
                targetDictionary[kvp.Key] = kvp.Value;
        }
    }
}
