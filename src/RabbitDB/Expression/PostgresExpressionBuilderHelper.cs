using System;
using RabbitDB.Storage;
using RabbitDB.SqlDialect;

namespace RabbitDB.Expressions
{
    internal class PostgresExpressionBuilderHelper : ExpressionBuildHelper
    {
        internal PostgresExpressionBuilderHelper(SqlCharacters sqlCharacters)
            : base(sqlCharacters) { }

        public override string FormatBoolean(bool value)
        {
            return value ? "'t'" : "'f'";
        }

        public override string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
                throw new ArgumentNullException("column");

            return string.Format("substring({0} from {1} for {2})", base.EscapeName(column), pos + 1, length);
        }

        public override string Length(string column)
        {
            return string.Format("char_length({0})", base.EscapeName(column));
        }

        //public override string Year(string column)
        //{
        //    return string.Format("extract(YEAR from {0})", base.EscapeName(column));
        //}

        //public override string Day(string column)
        //{
        //    return string.Format("extract(DAY from {0})", base.EscapeName(column));
        //}
    }
}