// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresExpressionBuilderHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The postgres expression builder helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.Expressions
{
    /// <summary>
    ///     The postgres expression builder helper.
    /// </summary>
    internal class PostgresExpressionBuilderHelper : ExpressionBuildHelper
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="PostgresExpressionBuilderHelper" /> class.
        /// </summary>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        internal PostgresExpressionBuilderHelper(ISqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
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
        public override string FormatBoolean(bool value)
        {
            return value
                ? "'t'"
                : "'f'";
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
        public override string Length(string column)
        {
            return $"char_length({EscapeName(column)})";
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
        public override string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException(nameof(column));
            }

            return $"substring({EscapeName(column)} from {pos + 1} for {length})";
        }

        #endregion
    }
}