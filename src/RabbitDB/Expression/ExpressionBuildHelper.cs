using RabbitDB.SqlDialect;
using System;

namespace RabbitDB.Expressions
{
    internal class ExpressionBuildHelper : IDbProviderExpressionBuildHelper
    {
        protected SqlCharacters _sqlCharacters;

        internal ExpressionBuildHelper(SqlCharacters sqlCharacters)
        {
            _sqlCharacters = sqlCharacters;
        }

        public virtual string FormatBoolean(bool value)
        {
            return value ? "1" : "0";
        }

        public virtual string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
                throw new ArgumentNullException("column");

            var idx = pos + 1;
            return string.Format("substring({0},{1},{2})", _sqlCharacters.EscapeName(column), idx, length);
        }

        public string ToUpper(string column)
        {
            return string.Format("upper({0})", _sqlCharacters.EscapeName(column));
        }

        public string ToLower(string column)
        {
            return string.Format("lower({0})", _sqlCharacters.EscapeName(column));
        }

        public virtual string Length(string column)
        {
            return string.Format("len({0})", _sqlCharacters.EscapeName(column));
        }

        public string EscapeName(string value)
        {
            return _sqlCharacters.EscapeName(value);
        }
    }
}