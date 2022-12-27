using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    internal static class LinqExtensions
    {
        public static T Cast<T>(this object obj)
        {
            if (obj is T value)
                return value;

            throw new InvalidCastException($"Casting {obj.GetType().Name} to {typeof(T).Name} is not supported.");
        }

        public static bool TryCast<T>(this object obj, out T value)
        {
            value = default;

            if (obj is T @out)
            {
                value = @out;
                return true;
            }

            return false;
        }

        public static IEnumerable<T> CastWhere<T>(this IEnumerable input)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    yield return @out;
        }

        public static IEnumerable<T> CastWhere<T>(this IEnumerable input, Func<T, bool> predicate)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    if (predicate(@out))
                        yield return @out;
        }

        public static IReadOnlyList<T> GetRange<T>(this IReadOnlyList<T> input, int startIndex, int? count = null)
        {
            IEnumerable<T> InnerGetRange()
            {
                count ??= (input.Count - startIndex);

                for (int i = startIndex; i <= count; i++)
                    yield return input[i];
            }

            var range = InnerGetRange();

            return range.ToList();
        }
    }
}
