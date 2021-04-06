using LCG.Template.Common.Extensions.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LCG.Template.Common.Extensions.PrimitiveTypes
{
    public static class StringExtensions
    {
        public static string ToUpperFirstLetter(this string str)
        {
            var stringArray = str.Split('.');
            for (int i = 0; i < stringArray.Length; i++)
            {
                stringArray[i] = char.ToUpper(stringArray[i][0]) + stringArray[i].Substring(1);
            }
            return string.Join('.', stringArray);
        }

        public static string Template(this string str, dynamic template)
        {

            var pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(str, pattern);

            Dictionary<string, string> values = new Dictionary<string, string>();
            GetDictionary(template, "", values);
            string value = "";
            foreach (Match match in matches)
            {
                var key = match.Value;
                var valueExists = values.TryGetValue(key, out value);
                if (valueExists)
                    str = str.Replace(key, value);
            }

            return str;
        }

        private static string GetDictionary(object template, string prefix, Dictionary<string, string> dictionary)
        {
            if (template == null)
                return null;

            if (template.GetType().IsValueType == false && template is string == false)
            {
                var ret =
                    ((object)template)
                    .GetType()
                    .GetProperties()
                    .ToDictionary(
                        p => "{" + prefix + p.Name + "}",
                        p => GetDictionary(p.GetValue(template), p.Name + ".", dictionary));
                dictionary.AddRange(ret);
            }
            return template.ToString();
        }
    }
}
