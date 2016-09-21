// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbCommandExecutor.cs" company="">
//   
// </copyright>
// <summary>
//   The db command executor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Storage;
using RabbitDB.Materialization;
using RabbitDB.Reader;

#endregion

namespace RabbitDB.Storage
{
    /// <summary>
    ///     The db command executor.
    /// </summary>
    internal class DbCommandExecutor : IDbCommandExecutor
    {
        #region Fields

        /// <summary>
        ///     The _db provider.
        /// </summary>
        private readonly IDbProvider _dbProvider;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbCommandExecutor" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        internal DbCommandExecutor(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbCommandExecutor" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        /// <param name="nullValueResolver">
        ///     The null value resolver.
        /// </param>
        internal DbCommandExecutor(IDbProvider dbProvider, INullValueResolver nullValueResolver)
            : this(dbProvider)
        {
            NullValueResolver = nullValueResolver;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the null value resolver.
        /// </summary>
        public INullValueResolver NullValueResolver { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The execute reader.
        /// </summary>
        /// <param name="dbCommand">
        ///     The db command.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public virtual IEntityReader<T> ExecuteReader<T>(IDbCommand dbCommand)
        {
            IDataReader dataReader = dbCommand.ExecuteReader();

            return new EntityReader<T>(dataReader, _dbProvider, new EntityMaterializer(NullValueResolver));
        }

        /// <summary>
        ///     The execute command.
        /// </summary>
        /// <param name="dbCommand">
        ///     The db command.
        /// </param>
        public void ExecuteCommand(IDbCommand dbCommand)
        {
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            finally
            {
                _dbProvider.Dispose();
            }
        }

        /// <summary>
        ///     The execute reader.
        /// </summary>
        /// <param name="dbCommand">
        ///     The db command.
        /// </param>
        /// <returns>
        ///     The <see cref="IDataReader" />.
        /// </returns>
        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteReader();
        }

        /// <summary>
        ///     The execute scalar.
        /// </summary>
        /// <param name="dbCommand">
        ///     The db command.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T ExecuteScalar<T>(IDbCommand dbCommand)
        {
            try
            {
                return (T)dbCommand.ExecuteScalar();
            }
            finally
            {
                _dbProvider.Dispose();
            }
        }

        #endregion
    }
}