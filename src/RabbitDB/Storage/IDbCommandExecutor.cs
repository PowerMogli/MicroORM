using RabbitDB.Reader;
using System.Data;

namespace RabbitDB.Storage
{
    interface IDbCommandExecutor
    {
        INullValueResolver NullValueResolver { get; set; }
        void ExecuteCommand(IDbCommand dbCommand);
        IDataReader ExecuteReader(IDbCommand dbCommand);
        EntityReader<T> ExecuteReader<T>(IDbCommand dbCommand);
        T ExecuteScalar<T>(IDbCommand dbCommand);
    }
}