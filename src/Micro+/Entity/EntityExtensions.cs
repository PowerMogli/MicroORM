using System.Data;
using MicroORM.Base;
using MicroORM.Caching;
using MicroORM.Storage;

namespace MicroORM.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            if (entityInfo.EntityState == EntityState.Loaded) return;

            string connectionString = Registrar<string>.GetFor(entity.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(entity.GetType());

            using (IDbSession entitySession = new DbSession(connectionString, dbEngine))
            {
                entitySession.Load(entity);
                entityInfo.EntityState = EntityState.Loaded;
                entityInfo.ComputeSnapshot(entity);
            }
        }

        public static bool PersistChanges<TEntity>(this TEntity entity, bool delete = false) where TEntity : Entity
        {
            string connectionString = Registrar<string>.GetFor(entity.GetType());
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(entity.GetType());
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);

            using (IDbSession dbSession = new DbSession(connectionString, dbEngine))
            {
                try
                {
                    using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        if (dbSession.PersistChanges(entity, delete) == false) return false;

                        transaction.Commit();
                        entityInfo.MergeChanges();
                        return true;
                    }
                }
                catch
                {
                    entityInfo.ClearChanges();
                }
                return false;
            }
        }
    }
}

