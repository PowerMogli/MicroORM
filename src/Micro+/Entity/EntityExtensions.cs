using System.Data;
using MicroORM.Base;

namespace MicroORM.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            using (IDbSession entitySession = entity.EntitySession)
            {
                entitySession.Load(entity);
                entity.EntityInfo.EntityState = EntityState.Loaded;
            }
        }

        public static void PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IDbSession dbSession = entity.EntitySession)
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

