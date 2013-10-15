using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MicroORM.Attributes;
using MicroORM.Mapping;
using MicroORM.Reflection;

namespace MicroORM.Utils
{
    internal static class Utils
    {
        internal static object[] GetEntityArguments<TEntity>(TEntity entity, TableInfo tableInfo)
        {
            KeyValuePair<string, object>[] properties = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });
            int count = properties.Count();
            List<object> arguments = new List<object>();
            for (int i = 0; i < count; i++)
            {
                IPropertyInfo memberInfo = tableInfo.Columns.Where(member => member.ColumnAttribute.ColumnName == properties[i].Key).First();
                if (tableInfo.PrimaryKeys.Contains(memberInfo.ColumnAttribute.ColumnName)
                    && ((ColumnAttribute)memberInfo.ColumnAttribute).AutoNumber) { continue; }

                arguments.Add(properties[i].Value);
            }
            return arguments.ToArray();
        }

        internal static bool IsCustomObject<T>(this T t)
        {
            return !(t is ValueType) && (Type.GetTypeCode(t.GetType()) == TypeCode.Object);
        }

        internal static bool IsListParam(this object data)
        {
            if (data == null) return false;
            return data is IEnumerable && !(data is string) && !(data is byte[]);
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

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
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
