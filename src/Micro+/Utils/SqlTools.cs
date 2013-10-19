using System;

namespace MicroORM.Utils
{
    internal class SqlTools
    {
        internal static T GetDbValue<T>(object value)
        {
            if (Convert.IsDBNull(value)) return default(T);

            return (T)value;
        }

        internal static int GetDatatypePrecision(string type)
        {
            int startPos = type.IndexOf(",");
            if (startPos < 0) return -1;

            int endPos = type.IndexOf(")");
            if (endPos < 0) return -1;

            string typePrecisionStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result = -1;
            if (int.TryParse(typePrecisionStr, out result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }

        internal static int GetDatatypeSize(string type)
        {
            int startPos = type.IndexOf("(");
            if (startPos < 0) return -1;

            int endPos = type.IndexOf(",");
            if (endPos < 0)
            {
                endPos = type.IndexOf(")");
            }

            string typeSizeStr = type.Substring(startPos + 1, endPos - startPos - 1);
            int result = -1;
            if (int.TryParse(typeSizeStr, out result))
            {
                return result;
            }
            else
            {
                return -1;
            }
        }
    }
}
