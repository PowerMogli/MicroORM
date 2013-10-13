using System.Data;
using MicroORM.Base;
using MicroORM.Storage;

namespace MicroORM.Query
{
    public static class ProcedureExtensions
    {
        public static TEntity Execute<TEntity>(this ProcedureObject procedureObject)
        {
            string connectionString = ConnectionStringRegistrar.GetFor(procedureObject.GetType());
            DbEngine dbEngine = DbEngineRegistrar.GetFor(procedureObject.GetType());
            using (IDbSession dbSession = new DbSession(connectionString, dbEngine))
            {
                return dbSession.ExecuteStoredProcedure<TEntity>(procedureObject);
            }
        }

        public static void Execute(this ProcedureObject procedureObject, IsolationLevel? isolationLevel = null)
        {
            string connectionString = ConnectionStringRegistrar.GetFor(procedureObject.GetType());
            DbEngine dbEngine = DbEngineRegistrar.GetFor(procedureObject.GetType());
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
