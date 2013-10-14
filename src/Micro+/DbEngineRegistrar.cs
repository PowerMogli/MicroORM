using System.Collections.Concurrent;
using MicroORM.Storage;

namespace MicroORM.Base
{
    public class DbEngineRegistrar : Registrar<DbEngine>
    {
        private DbEngineRegistrar()
        {
            _container = new ConcurrentDictionary<string, DbEngine>();
        }
    }
}
