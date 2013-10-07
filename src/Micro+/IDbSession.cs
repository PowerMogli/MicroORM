using System;
using System.Data;
using System.Linq.Expressions;
using MicroORM.Query;
using MicroORM.Storage;

namespace MicroORM.Base
{
    interface IDbSession
    {
        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);
        void ExecuteCommand(string sql, params object[] args);
        void ExecuteStoredProcedure(ProcedureObject procedureObject);
        void ExecuteStoredProcedure<T>(ProcedureObject procedureObject);
        V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria);
        T GetObject<T>(Expression<Func<T, bool>> criteria);
        T GetObject<T>(object primaryKey, string additionalPredicate = null, params object[] args);
        ObjectSet<T> GetObjectSet<T>();
        ObjectSet<T> GetObjectSet<T>(Expression<Func<T, bool>> condition);
        ObjectSet<T> GetObjectSet<T>(string sql, params object[] args);
        T GetValue<T>(string sql, params object[] args);
        bool PersistChanges();
        void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments);
        void Update<T>(T data);
    }
}
