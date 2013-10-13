using System.Collections.Concurrent;

namespace MicroORM.Base
{

    public class ConnectionStringRegistrar : Registrar<string>
    {
        private ConnectionStringRegistrar()
        {
            _container = new ConcurrentDictionary<string, string>();
        }
    }
}
