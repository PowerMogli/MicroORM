using System.Data;
using RabbitDB.Base;
using RabbitDB.Storage;

namespace RabbitDB.Query.StoredProcedure
{
    public static class ProcedureExtensions
    {
        public static TEntity Execute<TEntity>(this StoredProcedure procedureObject)
        {
            string connectionString = Registrar<string>.GetFor(procedureObject.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(procedureObject.GetType());
            using (IDbSession dbSession = new DbSession(connectionString, dbEngine))
            {
                return dbSession.ExecuteStoredProcedure<TEntity>(procedureObject);
            }
        }

        public static void Execute(this StoredProcedure procedureObject, IsolationLevel? isolationLevel = null)
        {
            string connectionString = Registrar<string>.GetFor(procedureObject.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(procedureObject.GetType());
            using (IDbSession dbSession = new DbSession(connectionString, dbEngine))
            {
                IDbTransaction transaction = null;
                if (isolationLevel != null)
                    transaction = dbSession.BeginTransaction(isolationLevel);

                dbSession.ExecuteStoredProcedure(procedureObject);

                if (transaction == null) return;
                transaction.Commit();
            }
        }
    }
}