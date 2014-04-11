// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The postgres db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    /// <summary>
    /// The postgres db provider.
    /// </summary>
    internal class PostgresDbProvider : TransactionalDbProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgresDbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        internal PostgresDbProvider(string connectionString)
            : base(connectionString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public override string ProviderName
        {
            get
            {
                return "Npgsql";
            }
        }

        #endregion
    }
}