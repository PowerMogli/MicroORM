using System;
using System.Linq;
using MicroORM.Mapping;
using MicroORM.Storage;

namespace MicroORM.Base
{
    public abstract class Entity
    {
        private IDbSession _dbSession;
        internal EntityState EntityState { get; set; }
        internal IDbSession DbSession
        {
            get { return _dbSession; }
            set { _dbSession = value; }
        }

        public Entity()
        {
            string connectionString = ConnectionStringRegistrar.GetFor("");
            DbEngine dbEngine = DbEngineRegistrar.GetFor("");
            _dbSession = new DbSession(connectionString, dbEngine);
        }

        public Entity(IDbSession dbSession)
        {
            _dbSession = dbSession;
            this.EntityState = EntityState.None;
        }
    }
}
