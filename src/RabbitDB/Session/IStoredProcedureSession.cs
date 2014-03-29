using RabbitDB.Base;
using RabbitDB.Query.StoredProcedure;
using System;

namespace RabbitDB
{
    internal interface IStoredProcedureSession : ITransactionalSession, IDisposable
    {
        void ExecuteStoredProcedure(StoredProcedure procedureObject);
        void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments);
        T ExecuteStoredProcedure<T>(StoredProcedure procedureObject);
        T ExecuteStoredProcedure<T>(string storedProcedureName, params object[] arguments);
    }
}