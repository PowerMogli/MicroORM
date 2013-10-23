using System.Data;
using MicroORM.Base;
using MicroORM.Storage;

namespace MicroORM.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            if (entity.EntityState == EntityState.Loaded) return;

            string connectionString = Registrar<string>.GetFor(entity.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(entity.GetType());

            using (IDbSession entitySession = new DbSession(connectionString, dbEngine))
            {
                entitySession.Load(entity);
                entity.EntityState = EntityState.Loaded;
            }
        }

        public static void PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity
        {
            string connectionString = Registrar<string>.GetFor(entity.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(entity.GetType());

            using (IDbSession dbSession = new DbSession(connectionString, dbEngine))
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.PersistChanges(entity);
                    transaction.Commit();
                }
            }
        }
    }
}

