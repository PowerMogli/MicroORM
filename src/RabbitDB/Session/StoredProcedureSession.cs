// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureSession.cs" company="">
//   
// </copyright>
// <summary>
//   The stored procedure session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq;

using RabbitDB.Contracts;
using RabbitDB.Contracts.Query.StoredProcedure;
using RabbitDB.Contracts.Session;
using RabbitDB.Query;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Session
{
    /// <summary>
    ///     The stored procedure session.
    /// </summary>
    public class StoredProcedureSession : BaseDbSession,
                                          IStoredProcedureSession
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoredProcedureSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="dbEngine">
        ///     The db engine.
        /// </param>
        public StoredProcedureSession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoredProcedureSession" /> class.
        /// </summary>
        /// <param name="assemblyType">
        ///     The assembly type.
        /// </param>
        public StoredProcedureSession(Type assemblyType)
            : base(assemblyType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoredProcedureSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        public StoredProcedureSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        public new void Dispose()
        {
        }

        /// <summary>
        ///     The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        ///     The procedure object.
        /// </param>
        public void ExecuteStoredProcedure(IStoredProcedure procedureObject)
        {
            SqlDialect.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        /// <summary>
        ///     The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        ///     The procedure object.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEntity" />.
        /// </returns>
        public TEntity ExecuteStoredProcedure<TEntity>(IStoredProcedure procedureObject)
        {
            IEntitySet<TEntity> objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(new StoredProcedureQuery(procedureObject));

            return objectSet.SingleOrDefault();
        }

        /// <summary>
        ///     The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        ///     The stored procedure name.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            SqlDialect.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
        }

        /// <summary>
        ///     The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        ///     The stored procedure name.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEntity" />.
        /// </returns>
        public TEntity ExecuteStoredProcedure<TEntity>(string storedProcedureName, params object[] arguments)
        {
            StoredProcedureQuery query = new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create<TEntity>(arguments));

            IEntitySet<TEntity> objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(query);

            return objectSet.SingleOrDefault();
        }

        #endregion
    }
}