using MicroORM.Storage;

namespace MicroORM.Base.Entity
{
    public abstract class Entity
    {
        internal EntityState EntityState { get; set; }
        internal IEntitySession EntitySession { get; set; }

        public Entity()
        {
            string connectionString = ConnectionStringRegistrar.GetFor(this.GetType());
            DbEngine dbEngine = DbEngineRegistrar.GetFor(this.GetType());
            this.EntitySession = new EntitySession(connectionString, dbEngine);
        }
    }
}
