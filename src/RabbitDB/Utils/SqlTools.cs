// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlTools.cs" company="">
//   
// </copyright>
// <summary>
//   The sql tools.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Utils
{
    using System;

    /// <summary>
    /// The sql tools.
    /// </summary>
    internal class SqlTools
    {
        #region Methods

        /// <summary>
        /// The get datatype precision.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int GetDatatypePrecision(string type)
        {
            var startPos = type.IndexOf(",", StringComparison.Ordinal);
            if (startPos < 0)
            {
                return -1;
            }

            var endPos = type.IndexOf(")", StringComparison.Ordinal);
            if (endPos < 0)
            {
                return -1;
            }

            var typePrecisionStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result;
            if (int.TryParse(typePrecisionStr, out result))
            {
                return result;
            }
            
            return -1;
        }

        /// <summary>
        /// The get datatype size.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int GetDatatypeSize(string type)
        {
            var startPos = type.IndexOf("(", StringComparison.Ordinal);
            if (startPos < 0)
            {
                return -1;
            }

            var endPos = type.IndexOf(",", StringComparison.Ordinal);
            if (endPos < 0)
            {
                endPos = type.IndexOf(")", StringComparison.Ordinal);
            }

            var typeSizeStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result;
            if (int.TryParse(typeSizeStr, out result))
            {
                return result;
            }
            return -1;
        }

        /// <summary>
        /// The get db value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        internal static T GetDbValue<T>(object value)
        {
            if (Convert.IsDBNull(value))
            {
                return default(T);
            }

            return (T)value;
        }

        #endregion
    }
}