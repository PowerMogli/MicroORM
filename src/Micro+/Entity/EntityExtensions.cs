using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;
using MicroORM.Caching;

namespace MicroORM.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            using (IDbSession entitySession = entity.EntitySession)
            {
                entitySession.Load(entity);
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

