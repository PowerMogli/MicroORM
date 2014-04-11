// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresExpressionBuilderHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The postgres expression builder helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Expressions
{
    using System;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The postgres expression builder helper.
    /// </summary>
    internal class PostgresExpressionBuilderHelper : ExpressionBuildHelper
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresExpressionBuilderHelper"/> class.
        /// </summary>
        /// <param name="sqlCharacters">
        /// The sql characters.
        /// </param>
        internal PostgresExpressionBuilderHelper(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format boolean.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string FormatBoolean(bool value)
        {
            return value ? "'t'" : "'f'";
        }

        /// <summary>
        /// The length.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string Length(string column)
        {
            return string.Format("char_length({0})", EscapeName(column));
        }

        /// <summary>
        /// The substring.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public override string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException("column");
            }

            return string.Format("substring({0} from {1} for {2})", EscapeName(column), pos + 1, length);
        }

        #endregion

        // public override string Year(string column)
        // {
        // return string.Format("extract(YEAR from {0})", base.EscapeName(column));
        // }

        // public override string Day(string column)
        // {
        // return string.Format("extract(DAY from {0})", base.EscapeName(column));
        // }
    }
}