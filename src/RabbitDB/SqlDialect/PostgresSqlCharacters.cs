
namespace RabbitDB.SqlDialect
{
    internal class PostgresSqlCharacters : SqlCharacters
    {
        internal override string ParameterPrefix
        {
            get
            {
                return ":";
            }
        }
    }
}