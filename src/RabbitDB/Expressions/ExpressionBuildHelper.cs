// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionBuildHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The expression build helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Contracts.Expressions;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.SqlDialect;

#endregion

namespace RabbitDB.Expressions
{
    /// <summary>
    ///     The expression build helper.
    /// </summary>
    internal class ExpressionBuildHelper : IDbProviderExpressionBuildHelper
    {
        #region Fields

        /// <summary>
        ///     The _sql characters.
        /// </summary>
        protected ISqlCharacters SqlCharacters;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionBuildHelper" /> class.
        /// </summary>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        internal ExpressionBuildHelper(ISqlCharacters sqlCharacters)
        {
            SqlCharacters = sqlCharacters;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The format boolean.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string FormatBoolean(bool value)
        {
            return value
                ? "1"
                : "0";
        }

        /// <summary>
        ///     The length.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public virtual string Length(string column)
        {
            return $"len({SqlCharacters.EscapeName(column)})";
        }

        /// <summary>
        ///     The substring.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="pos">
        ///     The pos.
        /// </param>
        /// <param name="length">
        ///     The length.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public virtual string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            int idx = pos + 1;

            return $"substring({SqlCharacters.EscapeName(column)},{idx},{length})";
        }

        /// <summary>
        ///     The escape name.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string EscapeName(string value)
        {
            return SqlCharacters.EscapeName(value);
        }

        /// <summary>
        ///     The to lower.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToLower(string column)
        {
            return $"lower({SqlCharacters.EscapeName(column)})";
        }

        /// <summary>
        ///     The to upper.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string ToUpper(string column)
        {
            return $"upper({SqlCharacters.EscapeName(column)})";
        }

        #endregion
    }
}