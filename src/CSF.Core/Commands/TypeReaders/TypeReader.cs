using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a generic <see cref="TypeReader{T}"/> to use for parsing provided types into the targetted type.
    /// </summary>
    /// <typeparam name="T">The targetted type for this typereader.</typeparam>
    public abstract class TypeReader<T> : ITypeReader
    {
        /// <inheritdoc />
        public abstract Task<TypeReaderResult> ReadAsync(IContext context, Parameter info, object value, IServiceProvider provider);
    }

    internal static class TypeReader
    {
        public static Dictionary<Type, ITypeReader> CreateDefaultReaders()
        {
            var dictionary = BaseTypeReader.CreateBaseReaders();

            dictionary.Add(typeof(TimeSpan), new TimeSpanTypeReader());
            dictionary.Add(typeof(Color), new ColorTypeReader());

            return dictionary;
        }
    }
}
