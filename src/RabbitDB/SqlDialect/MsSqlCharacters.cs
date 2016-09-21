// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlCharacters.cs" company="">
//   
// </copyright>
// <summary>
//   The ms sql characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    /// <summary>
    /// The ms sql characters.
    /// </summary>
    internal class MsSqlCharacters : SqlCharacters
    {
        #region Properties

        /// <summary>
        /// Gets the left delimiter.
        /// </summary>
        internal override string LeftDelimiter => "[";

        /// <summary>
        /// Gets the right delimiter.
        /// </summary>
        internal override string RightDelimiter => "]";

        #endregion
    }
}