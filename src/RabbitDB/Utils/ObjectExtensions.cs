// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The object extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Utils
{
    using System;
    using System.Collections;
    using System.Globalization;

    /// <summary>
    /// The object extensions.
    /// </summary>
    internal static class ObjectExtensions
    {
        #region Methods

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// </exception>
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
            if (otp == type)
            {
                return data;
            }

            if (type.IsEnum)
            {
                if (data is string)
                {
                    return Enum.Parse(type, data.ToString());
                }

                var o = Enum.ToObject(type, data);
                return o;
            }

            if (!type.IsValueType)
            {
                return type == typeof(CultureInfo) ? new CultureInfo(data.ToString()) : Convert.ChangeType(data, type);
            }

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
                if (!(data is DateTime))
                {
                    return DateTimeOffset.Parse(data.ToString());
                }

                var dt = (DateTime)data;
                return new DateTimeOffset(dt);
            }

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return Convert.ChangeType(data, type);
            }

            var under = Nullable.GetUnderlyingType(type);
            return data.ConvertTo(under);
        }

        /// <summary>
        /// The is custom object.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsCustomObject<T>(this T t)
        {
            return !(t is ValueType) && (Type.GetTypeCode(t.GetType()) == TypeCode.Object);
        }

        /// <summary>
        /// The is default.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsDefault<T>(this T value) where T : struct
        {
            var isDefault = value.Equals(default(T));

            return isDefault;
        }

        /// <summary>
        /// The is list param.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsListParam(this object data)
        {
            if (data == null)
            {
                return false;
            }

            return data is IEnumerable && !(data is string) && !(data is byte[]);
        }

        #endregion
    }
}