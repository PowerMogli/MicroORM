// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureSession.cs" company="">
//   
// </copyright>
// <summary>
//   The stored procedure session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using System;
    using System.Linq;

    using RabbitDB.Query;
    using RabbitDB.Query.StoredProcedure;
    using RabbitDB.Storage;

    /// <summary>
    /// The stored procedure session.
    /// </summary>
    public class StoredProcedureSession : BaseDbSession, IStoredProcedureSession
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureSession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="dbEngine">
        /// The db engine.
        /// </param>
        public StoredProcedureSession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureSession"/> class.
        /// </summary>
        /// <param name="assemblyType">
        /// The assembly type.
        /// </param>
        public StoredProcedureSession(Type assemblyType)
            : base(assemblyType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureSession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        public StoredProcedureSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public new void Dispose()
        {
        }

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        public void ExecuteStoredProcedure(StoredProcedure procedureObject)
        {
            this.SqlDialect.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity ExecuteStoredProcedure<TEntity>(StoredProcedure procedureObject)
        {
            var objectSet =
                ((IBaseDbSession)this).GetEntitySet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            this.SqlDialect.ExecuteCommand(
                new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
        }

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity ExecuteStoredProcedure<TEntity>(string storedProcedureName, params object[] arguments)
        {
            var query = new StoredProcedureQuery(
                storedProcedureName, 
                QueryParameterCollection.Create<TEntity>(arguments));

            var objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(query);
            return objectSet.SingleOrDefault();
        }

        #endregion
    }
}