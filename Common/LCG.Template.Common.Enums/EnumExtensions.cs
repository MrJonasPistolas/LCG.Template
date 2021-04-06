using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LCG.Template.Common.Enums
{
    public interface IEnumValue<T>
    {
        T Value
        {
            get;
        }
    }

    public class Description : Attribute, IEnumValue<string>
    {
        public Description(string value)
        {
            Value = value;
        }

        public Description(Type ResourceType, string Name)
        {
            var property = ResourceType.GetProperty(Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var value = property.GetValue(null, null);
            Value = value.ToString();
        }

        public string Value { get; }
    }

    public class IsMultipleAnswer : Attribute, IEnumValue<bool>
    {
        public IsMultipleAnswer(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }

    public static class StringEnum
    {
        public static Output GetEnumValue<Output, EnumClass>(Enum value) where EnumClass : class, IEnumValue<Output>
        {
            Output output = default;
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            EnumClass[] attrs = fi.GetCustomAttributes(typeof(EnumClass), false) as EnumClass[];
            if (attrs.Length > 0)
                output = attrs[0].Value;

            return output;
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
