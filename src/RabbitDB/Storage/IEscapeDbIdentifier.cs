// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEscapeDbIdentifier.cs" company="">
//   
// </copyright>
// <summary>
//   The EscapeDbIdentifier interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    /// <summary>
    /// The EscapeDbIdentifier interface.
    /// </summary>
    internal interface IEscapeDbIdentifier
    {
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
        string EscapeName(string value);

        #endregion
    }
}