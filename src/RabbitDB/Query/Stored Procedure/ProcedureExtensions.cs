// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcedureExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The procedure extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.StoredProcedure
{
    using System.Data;

    using RabbitDB.Session;
    using RabbitDB.Storage;

    /// <summary>
    /// The procedure extensions.
    /// </summary>
    public static class ProcedureExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public static TEntity Execute<TEntity>(this StoredProcedure procedureObject)
        {
            var connectionString = Registrar<string>.GetFor(procedureObject.GetType());
            var dbEngine = Registrar<DbEngine>.GetFor(procedureObject.GetType());
            using (IStoredProcedureSession dbSession = new StoredProcedureSession(connectionString, dbEngine))
            {
                return dbSession.ExecuteStoredProcedure<TEntity>(procedureObject);
            }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        public static void Execute(this StoredProcedure procedureObject, IsolationLevel? isolationLevel = null)
        {
            var connectionString = Registrar<string>.GetFor(procedureObject.GetType());
            var dbEngine = Registrar<DbEngine>.GetFor(procedureObject.GetType());
            using (IStoredProcedureSession dbSession = new StoredProcedureSession(connectionString, dbEngine))
            {
                IDbTransaction transaction = null;
                if (isolationLevel != null)
                {
                    transaction = dbSession.BeginTransaction(isolationLevel);
                }

                dbSession.ExecuteStoredProcedure(procedureObject);

                if (transaction == null)
                {
                    return;
                }

                transaction.Commit();
            }
        }

        #endregion
    }
}