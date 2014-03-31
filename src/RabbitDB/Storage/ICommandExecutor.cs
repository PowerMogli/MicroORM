using RabbitDB.Query;
using RabbitDB.Reader;
using System.Data;

namespace RabbitDB.Storage
{
    internal interface ICommandExecutor
    {
        void ExecuteCommand(IQuery query);
        IDataReader ExecuteReader(IQuery query);
        EntityReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}