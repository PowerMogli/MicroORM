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

        public Entity() { }

        public Entity(string connectionString, DbEngine dbEngine)
            : this(new DbSession(connectionString, dbEngine)) { }

        public Entity(IDbSession dbSession)
        {
            _dbSession = dbSession;
            this.EntityState = EntityState.None;
        }
    }
}
