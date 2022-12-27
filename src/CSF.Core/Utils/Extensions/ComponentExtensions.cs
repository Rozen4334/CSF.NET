using System.Collections.Generic;

namespace CSF
{
    internal static class ComponentExtensions
    {
        // todo: improve formatting
        public static string GetQualifiedNames(this IConditionalComponent component)
        {
            var values = new List<string>();

            if (component is ModuleInfo module)
                values.AddRange(GetRoot(module));
            else
                values.Add(component.Name);

            return string.Join(" -> ", values);
        }

        private static List<string> GetRoot(ModuleInfo components)
        {
            var values = new List<string>();

            foreach (var component in components.Components)
                if (component is ModuleInfo module)
                    values.AddRange(GetRoot(module));
                else
                    values.Add(component.Name);

            return values;
        }
    }
}
