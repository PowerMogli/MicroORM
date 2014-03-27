using System;
using System.Collections;
using System.Globalization;

namespace RabbitDB.Utils
{
    internal static class ObjectExtensions
    {
        internal static bool IsCustomObject<T>(this T t)
        {
            return !(t is ValueType) && (Type.GetTypeCode(t.GetType()) == TypeCode.Object);
        }

        internal static bool IsListParam(this object data)
        {
            if (data == null) return false;
            return data is IEnumerable && !(data is string) && !(data is byte[]);
        }

        internal static bool IsDefault<T>(this T value) where T : struct
        {
            bool isDefault = value.Equals(default(T));

            return isDefault;
        }

        internal static object ConvertTo(this object data, Type type)
        {
            if (data == null)
            {
                if (type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    throw new InvalidCastException("Cant convert null to a value type");
                }
                return null;
            }

            var otp = data.GetType();
            if (otp.Equals(type)) return data;
            if (type.IsEnum)
            {
                if (data is string)
                {
                    return Enum.Parse(type, data.ToString());
                }
                var o = Enum.ToObject(type, data);
                return o;
            }

            if (type.IsValueType)
            {
                if (type == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(data.ToString());
                }

                if (type == typeof(DateTime))
                {
                    if (data is DateTimeOffset)
                    {
                        return ((DateTimeOffset)data).DateTime;
                    }
                    return DateTime.Parse(data.ToString());
                }

                if (type == typeof(DateTimeOffset))
                {
                    if (data is DateTime)
                    {
                        var dt = (DateTime)data;
                        return new DateTimeOffset(dt);
                    }
                    return DateTimeOffset.Parse(data.ToString());
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var under = Nullable.GetUnderlyingType(type);
                    return data.ConvertTo(under);
                }
            }
            else if (type == typeof(CultureInfo)) return new CultureInfo(data.ToString());

            return Convert.ChangeType(data, type);
        }
    }
}
