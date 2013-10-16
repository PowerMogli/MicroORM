using System.Data;
using MicroORM.Storage;
using System;

namespace MicroORM.Query
{
    internal static class DbParameterExtension
    {
        internal static void Setup(this IDbDataParameter parameter, QueryParameter queryParameter, string parameterPrefix)
        {
            SetupParameter(parameter, queryParameter, parameterPrefix);
        }

        private static void SetupParameter(IDbDataParameter parameter, string parameterPrefix, string name, object value)
        {
            if (name == null) throw new ArgumentNullException("name", "Der Name des Parameters darf niemals NULL sein");

            parameter.ParameterName = string.Concat(parameterPrefix, name);
            if (value != null)
            {
                Type valueType = value.GetType();
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

        public static void SetupParameter(IDbDataParameter parameter, QueryParameter queryParameter, string parameterPrefix)
        {
            if ((queryParameter.DbType == DbType.AnsiString
                || queryParameter.DbType == DbType.AnsiStringFixedLength
                || queryParameter.DbType == DbType.String
                || queryParameter.DbType == DbType.StringFixedLength)
                && queryParameter.Size > 0
                && (queryParameter.Value is string && ((string)queryParameter.Value).Length > queryParameter.Size)) return;

            SetupParameter(parameter, parameterPrefix, queryParameter.Name, queryParameter.Value);

            DealWithSpecialParameterValues(parameter, queryParameter.Value, queryParameter.DbType, queryParameter.Size);
        }

        private static void DealWithSpecialParameterValues(IDbDataParameter parameter, object value, DbType dbType, int? size)
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
            else if (valueType.Name == "SqlGeometry") //SqlGeography is a CLR Type
            {
                dynamic param = parameter;
                param.UdtTypeName = "geometry";
            }
        }

        private static int GetStringSize(object value)
        {
            string strValue = value as string;

            if (string.IsNullOrEmpty(strValue))
                return 0;
            else if (strValue.Length < 4000)
                return strValue.Length;
            else
                return Math.Max(((string)value).Length + 1, 4000);
        }
    }
}
