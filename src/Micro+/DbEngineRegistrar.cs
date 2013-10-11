using MicroORM.Storage;

namespace MicroORM.Base
{
    public class DbEngineRegistrar : Registrar<DbEngine>
    {
        private DbEngineRegistrar()
        {
            _container = new System.Collections.Concurrent.ConcurrentDictionary<string, DbEngine>();
        }
    }
}
