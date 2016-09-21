// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbProviderExpressionBuildHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The DbProviderExpressionBuildHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using RabbitDB.Contracts.Storage;

namespace RabbitDB.Contracts.Expressions
{
    /// <summary>
    /// The DbProviderExpressionBuildHelper interface.
    /// </summary>
    internal interface IDbProviderExpressionBuildHelper : IEscapeDbIdentifier
    {
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
        string FormatBoolean(bool value);

        /// <summary>
        /// The length.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Length(string column);

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
        string Substring(string column, int pos, int length);

        /// <summary>
        /// The to lower.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ToLower(string column);

        /// <summary>
        /// The to upper.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ToUpper(string column);

        #endregion
    }
}