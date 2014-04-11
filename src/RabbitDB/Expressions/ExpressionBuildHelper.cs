// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionBuildHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The expression build helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Expressions
{
    using System;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The expression build helper.
    /// </summary>
    internal class ExpressionBuildHelper : IDbProviderExpressionBuildHelper
    {
        #region Fields

        /// <summary>
        /// The _sql characters.
        /// </summary>
        protected SqlCharacters SqlCharacters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuildHelper"/> class.
        /// </summary>
        /// <param name="sqlCharacters">
        /// The sql characters.
        /// </param>
        internal ExpressionBuildHelper(SqlCharacters sqlCharacters)
        {
            this.SqlCharacters = sqlCharacters;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The escape name.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string EscapeName(string value)
        {
            return this.SqlCharacters.EscapeName(value);
        }

        /// <summary>
        /// The format boolean.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string FormatBoolean(bool value)
        {
            return value ? "1" : "0";
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
        public virtual string Length(string column)
        {
            return string.Format("len({0})", this.SqlCharacters.EscapeName(column));
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
        public virtual string Substring(string column, int pos, int length)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new ArgumentNullException("column");
            }

            var idx = pos + 1;
            return string.Format("substring({0},{1},{2})", this.SqlCharacters.EscapeName(column), idx, length);
        }

        /// <summary>
        /// The to lower.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToLower(string column)
        {
            return string.Format("lower({0})", this.SqlCharacters.EscapeName(column));
        }

        /// <summary>
        /// The to upper.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToUpper(string column)
        {
            return string.Format("upper({0})", this.SqlCharacters.EscapeName(column));
        }

        #endregion
    }
}