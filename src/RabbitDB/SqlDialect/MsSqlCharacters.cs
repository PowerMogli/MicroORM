
namespace RabbitDB.SqlDialect
{
    internal class MsSqlCharacters : SqlCharacters
    {
        internal override string LeftDelimiter
        {
            get
            {
                return "[";
            }
        }

        internal override string RightDelimiter
        {
            get
            {
                return "]";
            }
        }
    }
}