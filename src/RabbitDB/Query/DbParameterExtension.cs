// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbParameterExtension.cs" company="">
//   
// </copyright>
// <summary>
//   The db parameter extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System;
    using System.Data;

    /// <summary>
    /// The db parameter extension.
    /// </summary>
    internal static class DbParameterExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// The setup parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="queryParameter">
        /// The query parameter.
        /// </param>
        /// <param name="parameterPrefix">
        /// The parameter prefix.
        /// </param>
        public static void SetupParameter(
            IDbDataParameter parameter, 
            QueryParameter queryParameter, 
            string parameterPrefix)
        {
            if ((queryParameter.DbType == DbType.AnsiString
                 || queryParameter.DbType == DbType.AnsiStringFixedLength
                 || queryParameter.DbType == DbType.String
                 || queryParameter.DbType == DbType.StringFixedLength)
                && queryParameter.Size > 0
                && (queryParameter.Value is string && ((string)queryParameter.Value).Length > queryParameter.Size))
            {
                return;
            }

            SetupParameter(parameter, parameterPrefix, queryParameter.Name, queryParameter.Value);

            DealWithSpecialParameterValues(parameter, queryParameter.Value, queryParameter.DbType, queryParameter.Size);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The setup.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="queryParameter">
        /// The query parameter.
        /// </param>
        /// <param name="parameterPrefix">
        /// The parameter prefix.
        /// </param>
        internal static void Setup(
            this IDbDataParameter parameter, 
            QueryParameter queryParameter, 
            string parameterPrefix)
        {
            SetupParameter(parameter, queryParameter, parameterPrefix);
        }

        /// <summary>
        /// The deal with special parameter values.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        private static void DealWithSpecialParameterValues(
            IDbDataParameter parameter, 
            object value, 
            DbType dbType, 
            int? size)
        {
            Type valueType = value.GetType();

            parameter.DbType = dbType;

            if (dbType == DbType.AnsiString
                || dbType == DbType.AnsiStringFixedLength
                || dbType == DbType.String
                || dbType == DbType.StringFixedLength)
            {
                parameter.Size = size ?? GetStringSize(value);
            }
            else if (valueType.Name == "SqlGeography")
            {
                dynamic param = parameter;
                param.UdtTypeName = "geography";
            }
            else if (valueType.Name == "SqlGeometry")
            {
                dynamic param = parameter;
                param.UdtTypeName = "geometry";
            }
        }

        /// <summary>
        /// The get string size.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetStringSize(object value)
        {
            var strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
            {
                return 0;
            }

            return strValue.Length < 4000 ? strValue.Length : Math.Max(((string)value).Length + 1, 4000);
        }

        /// <summary>
        /// The setup parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="parameterPrefix">
        /// The parameter prefix.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private static void SetupParameter(
            IDbDataParameter parameter, 
            string parameterPrefix, 
            string name, 
            object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "The name of a parameter can´t be NULL!");
            }

            if (name.StartsWith(parameterPrefix))
            {
                parameterPrefix = string.Empty;
            }

            parameter.ParameterName = string.Concat(parameterPrefix, name);

            if (value != null)
            {
                var valueType = value.GetType();
                if (valueType.IsEnum)
                {
                    if (value is string)
                    {
                        parameter.Value = Enum.Parse(valueType, value.ToString());
                    }

                    parameter.Value = Enum.ToObject(valueType, value);
                }
                else
                {
                    parameter.Value = value;
                }
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }

        #endregion
    }
}