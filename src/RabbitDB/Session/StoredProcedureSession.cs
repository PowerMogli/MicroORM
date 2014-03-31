using RabbitDB.Query;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Storage;
using System;
using System.Linq;

namespace RabbitDB.Base
{
    public class StoredProcedureSession : BaseDbSession, IStoredProcedureSession, IDisposable
    {
        #region Ctor

        public StoredProcedureSession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine) { }

        public StoredProcedureSession(Type assemblyType)
            : base(assemblyType) { }

        public StoredProcedureSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        #endregion

        public void ExecuteStoredProcedure(StoredProcedure procedureObject)
        {
            _sqlDialect.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(StoredProcedure procedureObject)
        {
            EntitySet<TEntity> objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            _sqlDialect.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(string storedProcedureName, params object[] arguments)
        {
            var query = new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create<TEntity>(arguments));
            EntitySet<TEntity> objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(query);
            return objectSet.SingleOrDefault();
        }
    }
}