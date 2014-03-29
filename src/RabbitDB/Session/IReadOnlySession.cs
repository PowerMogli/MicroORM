using RabbitDB.Query;
using RabbitDB.Reader;
using System;
using System.Linq.Expressions;

namespace RabbitDB.Base
{
    internal interface IReadOnlySession : ITransactionalSession, IDisposable
    {
        V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria);
        T GetEntity<T>(Expression<Func<T, bool>> criteria);
        T GetEntity<T>(object primaryKey, string additionalPredicate = null);
        T GetEntity<T>(object[] primaryKey, string additionalPredicate = null);
        EntitySet<T> GetEntitySet<T>();
        EntitySet<T> GetEntitySet<T>(Expression<Func<T, bool>> condition);
        EntitySet<T> GetEntitySet<T>(string sql, params object[] args);
        MultiEntityReader ExecuteMultiple(string sql, params object[] arguments);
        T GetScalarValue<T>(string sql, params object[] args);
        EntityReader<T> GetEntityReader<T>();
        EntityReader<T> GetEntityReader<T>(Expression<Func<T, bool>> condition);
        EntityReader<T> GetEntityReader<T>(string sql, params object[] args);
    }
}