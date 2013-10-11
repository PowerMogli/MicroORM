
using System.Data;
namespace MicroORM.Base
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            using (IDbSession dbSession = entity.DbSession)
            {
                dbSession.Load(entity);
                entity.EntityState = EntityState.Loaded;
            }
        }

        public static void Update<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IDbSession dbSession = entity.DbSession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Update(entity);
                    transaction.Commit();
                    entity.EntityState = EntityState.Updated;
                }
            }
        }

        public static void Insert<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IDbSession dbSession = entity.DbSession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Insert(entity);
                    transaction.Commit();
                    entity.EntityState = EntityState.Inserted;
                }
            }
        }
    }
}

