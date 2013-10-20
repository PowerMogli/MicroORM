using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;

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

        private static void Update<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IDbSession dbSession = entity.EntitySession)
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
            using (IDbSession dbSession = entity.EntitySession)
            {
                using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    dbSession.Insert(entity);
                    transaction.Commit();
                }
            }
        }

        public static void PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            if (entity.Delete)
                Delete(entity);
        }

        private static void Delete<TEntity>(this TEntity entity) where TEntity : Entity
        {
            using (IDbSession dbSession = entity.EntitySession)
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

