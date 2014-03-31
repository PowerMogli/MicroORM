using System;
using System.Linq;

namespace RabbitDB.SqlDialect
{
    internal class SqlCharacters
    {
        private static MsSqlCharacters _msSqlCharacters;
        private static PostgresSqlCharacters _postgresSqlCharacters;

        internal static SqlCharacters MsSqlCharacters
        {
            get
            {
                return _msSqlCharacters ?? (_msSqlCharacters = new MsSqlCharacters());
            }
        }

        internal static SqlCharacters PostgreSqlCharacters
        {
            get
            {
                return _postgresSqlCharacters ?? (_postgresSqlCharacters = new PostgresSqlCharacters());
            }
        }

        internal string EscapeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value", "Escaped value can´t be null!");

            if (IsEscaped(value))
            {
                return value;
            }

            if (!value.Contains("."))
                return this.LeftDelimiter + value + this.RightDelimiter;

            return string.Join(".", value.Split('.').Select(d => this.LeftDelimiter + d + this.RightDelimiter));
        }

        /// <summary>
        /// Determines whether the specified SQL is escaped.
        /// </summary>
        /// <param name="value">The SQL to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified SQL is escaped; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEscaped(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.StartsWith(this.LeftDelimiter, StringComparison.Ordinal) && value.EndsWith(this.RightDelimiter, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the SQL parameter prefix.
        /// </summary>
        internal virtual string ParameterPrefix
        {
            get
            {
                return "@";
            }
        }

        /// <summary>
        /// Gets the character used to separate SQL statements.
        /// </summary>
        internal virtual string StatementSeparator
        {
            get
            {
                return ";";
            }
        }

        /// <summary>
        /// Gets the left delimiter character.
        /// </summary>
        internal virtual string LeftDelimiter
        {
            get
            {
                return "\"";
            }
        }

        /// <summary>
        /// Gets the left delimiter character.
        /// </summary>
        internal virtual string RightDelimiter
        {
            get
            {
                return "\"";
            }
        }
    }
}