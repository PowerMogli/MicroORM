
namespace MicroORM.Base
{
    public static class EntityExtensions
    {
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity, new()
        {
            IDbSession dbSession = entity.DbSession;
            entity = dbSession.Load(entity);
            entity.DbSession = dbSession;
            entity.EntityState = EntityState.Loaded;
        }

        public static void Update<TEntity>(this TEntity entity) where TEntity : Entity
        {
            IDbSession dbSession = entity.DbSession;
            dbSession.Update(entity);
        }

        public static void Insert<TEntity>(this TEntity entity) where TEntity : Entity
        {
            IDbSession dbSession = entity.DbSession;
            dbSession.Insert(entity);
        }
    }
}
