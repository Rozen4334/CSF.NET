using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    internal class ColorTypeReader : TypeReader<Color>
    {
        private readonly Dictionary<string, Color> _colors;
        private readonly IReadOnlyDictionary<string, string> _spacedColors;

        public ColorTypeReader()
        {
            var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            var colors = new Dictionary<string, Color>(properties.Length - 1, StringComparer.OrdinalIgnoreCase);

            foreach (var property in properties)
                colors[property.Name] = (Color)property.GetValue(null);

            _colors = colors;

            var spacedNames = new Dictionary<string, string>(_colors.Comparer);
            var builder = new StringBuilder();
            foreach (var name in _colors.Keys)
            {
                builder.Clear();
                var hasSpace = false;
                for (var i = 0; i < name.Length; i++)
                {
                    var character = name[i];
                    if (i != 0 && char.IsUpper(character))
                    {
                        hasSpace = true;
                        builder.Append(' ');
                    }

                    builder.Append(character);
                }

                if (hasSpace)
                    spacedNames[builder.ToString()] = name;
            }

            _spacedColors = spacedNames;
        }

        public override ValueTask<TypeReaderResult> ReadAsync(IContext context, ParameterInfo parameter, object value, CancellationToken cancellationToken)
        {
            var str = value.ToString();
            if (int.TryParse(str.Replace("#", "").Replace("0x", ""), NumberStyles.HexNumber, null, out var hexNumber))
                return TypeReaderResult.FromSuccess(Color.FromArgb(hexNumber));

            var name = str;

            _spacedColors.TryGetValue(name, out name);

            if (_colors.TryGetValue(name, out var color))
                return TypeReaderResult.FromSuccess(color);

            return TypeReaderResult.FromError($"The provided value is not a color. Expected {typeof(Color).Name}, got: '{str}'. At: '{parameter.Name}'");
        }
    }
}
