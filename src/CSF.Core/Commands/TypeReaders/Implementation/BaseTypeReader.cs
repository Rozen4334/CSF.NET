using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    internal class BaseTypeReader<T> : TypeReader<T>
    {
        private delegate bool Tpd<TValue>(string str, out TValue value);

        private readonly static Lazy<IReadOnlyDictionary<Type, Delegate>> _container = new Lazy<IReadOnlyDictionary<Type, Delegate>>(ValueGenerator);

        public override ValueTask<TypeReaderResult> ReadAsync(IContext context, ParameterInfo parameter, object value, CancellationToken cancellationToken)
        {
            if (TryGetParser(out var parser))
            {
                if (parser(value.ToString(), out var result))
                    return TypeReaderResult.FromSuccess(result);
            }
            return TypeReaderResult.FromError($"The provided value does not match the expected type. Expected {typeof(T).Name}, got {value}. At: '{parameter.Name}'");
        }

        private static bool TryGetParser(out Tpd<T> parser)
        {
            parser = null;
            if (_container.Value.TryGetValue(typeof(T), out var result))
            {
                parser = (Tpd<T>)result;
                return true;
            }
            return false;
        }

        private static IReadOnlyDictionary<Type, Delegate> ValueGenerator()
        {
            var callback = new Dictionary<Type, Delegate>
            {
                // char
                [typeof(char)] = (Tpd<char>)char.TryParse,

                // bit / boolean
                [typeof(bool)] = (Tpd<bool>)bool.TryParse,

                // 8 bit int
                [typeof(byte)] = (Tpd<byte>)byte.TryParse,
                [typeof(sbyte)] = (Tpd<sbyte>)sbyte.TryParse,

                // 16 bit int
                [typeof(short)] = (Tpd<short>)short.TryParse,
                [typeof(ushort)] = (Tpd<ushort>)ushort.TryParse,

                // 32 bit int
                [typeof(int)] = (Tpd<int>)int.TryParse,
                [typeof(uint)] = (Tpd<uint>)uint.TryParse,

                // 64 bit int
                [typeof(long)] = (Tpd<long>)long.TryParse,
                [typeof(ulong)] = (Tpd<ulong>)ulong.TryParse,

                // floating point int
                [typeof(float)] = (Tpd<float>)float.TryParse,
                [typeof(double)] = (Tpd<double>)double.TryParse,
                [typeof(decimal)] = (Tpd<decimal>)decimal.TryParse,

                // time
                [typeof(DateTime)] = (Tpd<DateTime>)DateTime.TryParse,
                [typeof(DateTimeOffset)] = (Tpd<DateTimeOffset>)DateTimeOffset.TryParse,

                // guid
                [typeof(Guid)] = (Tpd<Guid>)Guid.TryParse
            };

            return callback;
        }
    }

    internal static class BaseTypeReader
    {
        public static Dictionary<Type, ITypeReader> CreateBaseReaders()
        {
            var callback = new Dictionary<Type, ITypeReader>
            {
                // char
                [typeof(char)] = new BaseTypeReader<char>(),

                // bit / boolean
                [typeof(bool)] = new BaseTypeReader<bool>(),

                // 8 bit int
                [typeof(byte)] = new BaseTypeReader<byte>(),
                [typeof(sbyte)] = new BaseTypeReader<sbyte>(),

                // 16 bit int
                [typeof(short)] = new BaseTypeReader<short>(),
                [typeof(ushort)] = new BaseTypeReader<ushort>(),

                // 32 bit int
                [typeof(int)] = new BaseTypeReader<int>(),
                [typeof(uint)] = new BaseTypeReader<uint>(),

                // 64 bit int
                [typeof(long)] = new BaseTypeReader<long>(),
                [typeof(ulong)] = new BaseTypeReader<ulong>(),

                // floating point int
                [typeof(float)] = new BaseTypeReader<float>(),
                [typeof(double)] = new BaseTypeReader<double>(),
                [typeof(decimal)] = new BaseTypeReader<decimal>(),

                // time
                [typeof(DateTime)] = new BaseTypeReader<DateTime>(),
                [typeof(DateTimeOffset)] = new BaseTypeReader<DateTimeOffset>(),

                // guid
                [typeof(Guid)] = new BaseTypeReader<Guid>()
            };

            return callback;
        }
    }
}
