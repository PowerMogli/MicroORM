using System;
using System.Data;
using RabbitDB.Base;
using RabbitDB.Caching;
using RabbitDB.Query;
using RabbitDB.Reader;
using RabbitDB.Storage;

namespace RabbitDB.Entity
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            if (entity.IsLoaded()) return;

            var sessionConfig = InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                dbSession.Load(entity);

                FinishLoad(entity);
            }
        }

        public static void Load<TEntity>(this TEntity entity, Action<TEntity, IDataReader> customMapper) where TEntity : Entity, new()
        {
            if (entity.IsLoaded()) return;

            var sessionConfig = InitializeSession<TEntity>();

            using (IBaseDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entityReader = dbSession.GetEntityReader<TEntity>(new EntityQuery<TEntity>(entity));
                entityReader.Load(entity, customMapper);

                FinishLoad(entity);
            }
        }

        internal static void FinishLoad<TEntity>(TEntity entity) where TEntity : Entity
        {
            if (entity.EntityInfo == null)
            {
                entity.EntityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            }
            if (entity.EntityInfo.EntityState == EntityState.Loaded) return;

            entity.EntityInfo.EntityState = EntityState.Loaded;
            entity.EntityInfo.ComputeSnapshot(entity);
        }

        internal static Tuple<string, DbEngine> InitializeSession<TEntity>()
        {
            var connectionString = Registrar<string>.GetFor(typeof(TEntity));
            var dbEngine = Registrar<DbEngine>.GetFor(typeof(TEntity));

            return new Tuple<string, DbEngine>(connectionString, dbEngine);
        }

        public static void MarkForDeletion<TEntity>(this TEntity entity) where TEntity : Entity
        {
            entity.MarkedForDeletion = true;
        }

        public static bool PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity
        {
            if (DbSession.Configuration.AutoDetectChangesEnabled == false)
                throw new InvalidOperationException("This operation is not allowed because change tracking has been disabled.");

            var sessionConfig = InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                try
                {
                    using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        if (dbSession.PersistChanges(entity) == false) return false;

                        transaction.Commit();
                        entity.EntityInfo.MergeChanges();
                        return true;
                    }
                }
                catch
                {
                    entity.EntityInfo.ClearChanges();
                }
                return false;
            }
        }
    }
}