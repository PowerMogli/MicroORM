// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlCharacters.cs" company="">
//   
// </copyright>
// <summary>
//   The sql characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq;

using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.SqlDialect
{
    /// <summary>
    ///     The sql characters.
    /// </summary>
    internal class SqlCharacters : ISqlCharacters
    {
        #region Fields

        /// <summary>
        ///     The _ms sql characters.
        /// </summary>
        private static MsSqlCharacters _msSqlCharacters;

        /// <summary>
        ///     The _postgres sql characters.
        /// </summary>
        private static PostgresSqlCharacters _postgresSqlCharacters;

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the left delimiter character.
        /// </summary>
        internal virtual string LeftDelimiter => "\"";

        /// <summary>
        ///     Gets the ms sql characters.
        /// </summary>
        internal static SqlCharacters MsSqlCharacters => _msSqlCharacters ?? (_msSqlCharacters = new MsSqlCharacters());

        /// <summary>
        ///     Gets the SQL parameter prefix.
        /// </summary>
        public virtual string ParameterPrefix => "@";

        /// <summary>
        ///     Gets the postgre sql characters.
        /// </summary>
        internal static SqlCharacters PostgreSqlCharacters => _postgresSqlCharacters ?? (_postgresSqlCharacters = new PostgresSqlCharacters());

        /// <summary>
        ///     Gets the left delimiter character.
        /// </summary>
        internal virtual string RightDelimiter => "\"";

        /// <summary>
        ///     Gets the character used to separate SQL statements.
        /// </summary>
        internal virtual string StatementSeparator => ";";

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The escape name.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public string EscapeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Escaped value can´t be null!");
            }

            if (IsEscaped(value))
            {
                return value;
            }

            if (!value.Contains("."))
            {
                return LeftDelimiter + value + RightDelimiter;
            }

            return string.Join(".", value.Split('.')
                                         .Select(d => LeftDelimiter + d + RightDelimiter));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the specified SQL is escaped.
        /// </summary>
        /// <param name="value">
        ///     The SQL to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified SQL is escaped; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEscaped(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.StartsWith(LeftDelimiter, StringComparison.Ordinal)
                   && value.EndsWith(RightDelimiter, StringComparison.Ordinal);
        }

        #endregion
    }
}