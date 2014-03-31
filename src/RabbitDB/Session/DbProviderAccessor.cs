using RabbitDB.Storage;

namespace RabbitDB.Session
{
    internal static class DbProviderAccessor
    {
        internal static SqlDialect.SqlDialect SqlDialect { get; set; }
    }
}