using MicroORM.Base;
using MicroORM.Storage;

namespace MicroORM.Entity
{
    public abstract class Entity
    {
        internal IEntitySession EntitySession { get; set; }

        public Entity()
        {
            string connectionString = ConnectionStringRegistrar.GetFor(this.GetType());
            DbEngine dbEngine = DbEngineRegistrar.GetFor(this.GetType());
            this.EntitySession = new EntitySession(connectionString, dbEngine);
        }
    }
}
