using System.Data;

namespace MicroORM.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            using (IEntitySession entitySession = entity.EntitySession)
            {
                entitySession.Load(entity);
            }
        }

        private static void Update<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IEntitySession dbSession = entity.EntitySession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Update(entity);
                    transaction.Commit();
                }
            }
        }

        private static void Insert<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IEntitySession dbSession = entity.EntitySession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Insert(entity);
                    transaction.Commit();
                }
            }
        }

        public static void Save<TEntity>(this TEntity entity) where TEntity : Entity
        {
        }

        public static void Delete<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IEntitySession dbSession = entity.EntitySession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Delete(entity);
                    transaction.Commit();
                }
            }
        }
    }
}

