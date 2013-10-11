using System;
using System.Linq;
using MicroORM.Mapping;
using MicroORM.Storage;

namespace MicroORM.Base
{
    public abstract class Entity
    {
        internal EntityState EntityState { get; set; }
        internal IDbSession DbSession
        {
            get;
            set;
        }

        public Entity()
        {
            string connectionString = ConnectionStringRegistrar.GetFor("");
            DbEngine dbEngine = DbEngineRegistrar.GetFor("");
            this.DbSession = new DbSession(connectionString, dbEngine);
        }

        public Entity(IDbSession dbSession)
        {
            this.DbSession = dbSession;
            this.EntityState = EntityState.None;
        }
    }
}
