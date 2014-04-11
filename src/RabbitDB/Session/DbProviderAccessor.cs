// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProviderAccessor.cs" company="">
//   
// </copyright>
// <summary>
//   The db provider accessor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The db provider accessor.
    /// </summary>
    internal static class DbProviderAccessor
    {
        #region Properties

        /// <summary>
        /// Gets or sets the sql dialect.
        /// </summary>
        internal static SqlDialect SqlDialect { get; set; }

        #endregion
    }
}