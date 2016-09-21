// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The sql db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    /// <summary>
    /// The sql db provider.
    /// </summary>
    internal class SqlDbProvider : TransactionalDbProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        internal SqlDbProvider(string connectionString)
            : base(connectionString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public override string ProviderName => "System.Data.SqlClient";

        #endregion
    }
}