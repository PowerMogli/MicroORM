using RabbitDB.Storage;

namespace RabbitDB.SqlDialect
{
    internal class MsSqlDialect : SqlDialect
    {
        internal MsSqlDialect(IDbProvider dbProvider)
            : base(SqlCharacter.MsSqlCharacter, dbProvider)
        {

        }
    }
}