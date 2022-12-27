using System;
using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a dictionary of type readers.
    /// </summary>
    public sealed class TypeReaderProvider
    {
        private readonly Dictionary<Type, ITypeReader> _typeReaders;

        /// <summary>
        ///     Creates a new typereader dictionary with all default readers.
        /// </summary>
        public TypeReaderProvider()
            : this(TypeReader.CreateDefaultReaders())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderProvider"/> with self-defined default readers.
        /// </summary>
        /// <param name="dictionary"></param>
        public TypeReaderProvider(Dictionary<Type, ITypeReader> dictionary)
        {
            _typeReaders = dictionary;
        }

        /// <summary>
        ///     Gets or sets a typereader for the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITypeReader this[Type key]
        {
            get
                => _typeReaders[key];
            set
                => _typeReaders[key] = value;
        }

        /// <summary>
        ///     Includes an <see cref="ITypeReader"/> in the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Include(ITypeReader reader)
        {
            _typeReaders.Add(reader.Type, reader);
            return this;
        }

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</typeparam>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Exclude<T>()
            => Exclude(typeof(T));

        /// <summary>
        ///     Excludes an <see cref="ITypeReader"/> from the <see cref="TypeReaderProvider"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to remove the <see cref="ITypeReader"/>.</param>
        /// <returns>The same instance for chaining calls.</returns>
        public TypeReaderProvider Exclude(Type type)
        {
            _typeReaders.Remove(type);
            return this;
        }

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetReader<T>(out ITypeReader reader)
            => TryGetReader(typeof(T), out reader);

        /// <summary>
        ///     Checks if the underlying dictionary contains the provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if success. False if not.</returns>
        public bool ContainsType(Type type)
            => _typeReaders.ContainsKey(type);

        /// <summary>
        ///     Tries to get a <see cref="ITypeReader"/> from the underlying dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns>True if success. False if not.</returns>
        public bool TryGetReader(Type type, out ITypeReader reader)
        {
            reader = null;

            if (type == typeof(string))
                return true;

            if (type == typeof(object))
                return true;

            if (_typeReaders.ContainsKey(type))
            {
                reader = _typeReaders[type];
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Copies all keys in the current dictionary to another, overwriting existing keys.
        /// </summary>
        /// <param name="targetDictionary">The target dictionary to copy to.</param>
        public void CopyTo(TypeReaderProvider targetDictionary)
        {
            foreach (var kvp in _typeReaders)
                targetDictionary[kvp.Key] = kvp.Value;
        }
    }
}
