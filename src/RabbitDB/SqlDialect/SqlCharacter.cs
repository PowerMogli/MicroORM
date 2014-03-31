
namespace RabbitDB.SqlDialect
{
    internal class SqlCharacter
    {
        internal static SqlCharacter MsSqlCharacter { get { return new MsSqlCharacter(); } }
    }
}