using RabbitDB.Storage;

namespace RabbitDB.Session
{
    internal static class SqlDbProviderAccessor
    {
        internal static IDbProvider DbProvider { get; set; }
    }
}