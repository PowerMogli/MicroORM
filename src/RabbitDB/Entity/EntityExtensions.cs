using System;
using System.Data;
using RabbitDB.Base;
using RabbitDB.Caching;
using RabbitDB.Query;
using RabbitDB.Storage;

namespace RabbitDB.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            if (entityInfo.EntityState == EntityState.Loaded) return;

            Tuple<string, DbEngine> result = InitializeSession<TEntity>();

            using (IDbSession entitySession = new DbSession(result.Item1, result.Item2))
            {
                entitySession.Load(entity);

                entityInfo.EntityState = EntityState.Loaded;
                entityInfo.ComputeSnapshot(entity);
            }
        }

        public static void Load<TEntity>(this TEntity entity, Action<TEntity, IDataReader> customMapper) where TEntity : Entity, new()
        {
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            if (entityInfo.EntityState == EntityState.Loaded) return;

            Tuple<string, DbEngine> result = InitializeSession<TEntity>();

            using (IDbSession entitySession = new DbSession(result.Item1, result.Item2))
            {
                EntityReader<TEntity> entityReader = entitySession.GetEntityReader<TEntity>(new EntityQuery<TEntity>(entity));
                entityReader.Load(entity, customMapper);

                entityInfo.EntityState = EntityState.Loaded;
                entityInfo.ComputeSnapshot(entity);
            }
        }

        internal static Tuple<string, DbEngine> InitializeSession<TEntity>()
        {
            string connectionString = Registrar<string>.GetFor(typeof(TEntity));
            DbEngine dbEngine = Registrar<DbEngine>.GetFor(typeof(TEntity));

            return new Tuple<string, DbEngine>(connectionString, dbEngine);
        }

        public static void MarkForDeletion<TEntity>(this TEntity entity) where TEntity : Entity
        {
            entity.MarkedForDeletion = true;
        }

        public static bool PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity
        {
            Tuple<string, DbEngine> result = InitializeSession<TEntity>();
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);

            using (IDbSession dbSession = new DbSession(result.Item1, result.Item2))
            {
                try
                {
                    using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        if (dbSession.PersistChanges(entity) == false) return false;

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

