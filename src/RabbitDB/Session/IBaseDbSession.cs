using RabbitDB.Query;
using RabbitDB.Reader;
using System;

namespace RabbitDB.Base
{
    internal interface IBaseDbSession : ITransactionalSession, IDisposable
    {
        EntitySet<T> GetEntitySet<T>(IQuery query);
        EntityReader<T> GetEntityReader<T>(IQuery query);
    }
}