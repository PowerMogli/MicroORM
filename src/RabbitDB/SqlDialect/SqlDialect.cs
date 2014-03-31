using RabbitDB.Storage;

namespace RabbitDB.SqlDialect
{
    internal abstract class SqlDialect
    {
        internal SqlCharacter SqlCharacter { get; set; }

        protected IDbProvider DbProvider { get; set; }

        internal SqlDialect(SqlCharacter sqlCharacter, IDbProvider dbProvider)
        {
            this.DbProvider = dbProvider;
            this.SqlCharacter = sqlCharacter;
        }
    }
}