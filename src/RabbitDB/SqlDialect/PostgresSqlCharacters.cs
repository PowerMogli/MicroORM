// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresSqlCharacters.cs" company="">
//   
// </copyright>
// <summary>
//   The postgres sql characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    /// <summary>
    /// The postgres sql characters.
    /// </summary>
    internal class PostgresSqlCharacters : SqlCharacters
    {
        #region Properties

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        internal override string ParameterPrefix
        {
            get
            {
                return ":";
            }
        }

        #endregion
    }
}