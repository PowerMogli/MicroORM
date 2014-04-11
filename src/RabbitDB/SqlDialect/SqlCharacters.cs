// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlCharacters.cs" company="">
//   
// </copyright>
// <summary>
//   The sql characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    using System;
    using System.Linq;

    /// <summary>
    /// The sql characters.
    /// </summary>
    internal class SqlCharacters
    {
        #region Static Fields

        /// <summary>
        /// The _ms sql characters.
        /// </summary>
        private static MsSqlCharacters msSqlCharacters;

        /// <summary>
        /// The _postgres sql characters.
        /// </summary>
        private static PostgresSqlCharacters postgresSqlCharacters;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ms sql characters.
        /// </summary>
        internal static SqlCharacters MsSqlCharacters
        {
            get
            {
                return msSqlCharacters ?? (msSqlCharacters = new MsSqlCharacters());
            }
        }

        /// <summary>
        /// Gets the postgre sql characters.
        /// </summary>
        internal static SqlCharacters PostgreSqlCharacters
        {
            get
            {
                return postgresSqlCharacters ?? (postgresSqlCharacters = new PostgresSqlCharacters());
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
        /// Gets the left delimiter character.
        /// </summary>
        internal virtual string RightDelimiter
        {
            get
            {
                return "\"";
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

        #endregion

        #region Methods

        /// <summary>
        /// The escape name.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal string EscapeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("value", "Escaped value can´t be null!");
            }

            if (this.IsEscaped(value))
            {
                return value;
            }

            if (!value.Contains("."))
            {
                return this.LeftDelimiter + value + this.RightDelimiter;
            }

            return string.Join(".", value.Split('.').Select(d => this.LeftDelimiter + d + this.RightDelimiter));
        }

        /// <summary>
        /// Determines whether the specified SQL is escaped.
        /// </summary>
        /// <param name="value">
        /// The SQL to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified SQL is escaped; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEscaped(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.StartsWith(this.LeftDelimiter, StringComparison.Ordinal)
                   && value.EndsWith(this.RightDelimiter, StringComparison.Ordinal);
        }

        #endregion
    }
}