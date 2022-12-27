using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    internal class TimeSpanTypeReader : TypeReader<TimeSpan>
    {
        private readonly IReadOnlyDictionary<string, Func<string, TimeSpan>> _callback;
        private readonly Regex _regex = new Regex(@"(\d*)\s*([a-zA-Z]*)\s*(?:and|,)?\s*", RegexOptions.Compiled);

        public TimeSpanTypeReader()
        {
            _callback = new Dictionary<string, Func<string, TimeSpan>>
            {
                ["second"] = Seconds,
                ["seconds"] = Seconds,
                ["sec"] = Seconds,
                ["s"] = Seconds,
                ["minute"] = Minutes,
                ["minutes"] = Minutes,
                ["min"] = Minutes,
                ["m"] = Minutes,
                ["hour"] = Hours,
                ["hours"] = Hours,
                ["h"] = Hours,
                ["day"] = Days,
                ["days"] = Days,
                ["d"] = Days,
                ["week"] = Weeks,
                ["weeks"] = Weeks,
                ["w"] = Weeks,
                ["month"] = Months,
                ["months"] = Months
            };
        }

        public override ValueTask<TypeReaderResult> ReadAsync(IContext context, ParameterInfo parameter, object value, CancellationToken cancellationToken)
        {
            var str = value.ToString();
            if (!TimeSpan.TryParse(str, out TimeSpan span))
            {
                str = str.ToLower().Trim();
                MatchCollection matches = _regex.Matches(str);
                if (matches.Count != 0)
                {
                    foreach (Match match in matches)
                        if (_callback.TryGetValue(match.Groups[2].Value, out var result))
                            span += result(match.Groups[1].Value);
                }
                else
                    return TypeReaderResult.FromError($"The provided value is no timespan. Expected {typeof(TimeSpan).Name}, got: '{str}'. At: '{parameter.Name}'");
            }

            return TypeReaderResult.FromSuccess(span);
        }

        private static TimeSpan Seconds(string match)
            => new TimeSpan(0, 0, int.Parse(match));

        private static TimeSpan Minutes(string match)
            => new TimeSpan(0, int.Parse(match), 0);

        private static TimeSpan Hours(string match)
            => new TimeSpan(int.Parse(match), 0, 0);

        private static TimeSpan Days(string match)
            => new TimeSpan(int.Parse(match), 0, 0, 0);

        private static TimeSpan Weeks(string match)
            => new TimeSpan((int.Parse(match) * 7), 0, 0, 0);

        private static TimeSpan Months(string match)
            => new TimeSpan(((int)(int.Parse(match) * 30.437)), 0, 0, 0);
    }
}
