using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.SqlBuilder;
using RabbitDB.Utils;

namespace RabbitDB.Storage
{
    internal class DbPersister : IDbPersister
    {
        private ICommandExecutor _dbCommandExecutor;

        internal DbPersister(ICommandExecutor dbCommandExecutor)
        {
            _dbCommandExecutor = dbCommandExecutor;
        }

        public void Update<TEntity>(IQuery query)
        {
            _dbCommandExecutor.ExecuteCommand(query);
        }

        public void Delete<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string deleteStatement = SqlBuilder<TEntity>.DeleteStatement;
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(tableInfo.GetPrimaryKeyValues(entity));

            _dbCommandExecutor.ExecuteCommand(new SqlQuery(deleteStatement, arguments));
        }

        public void Insert<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string insertStatement = SqlBuilder<TEntity>.InsertStatement;
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(new EntityArgumentsReader().GetEntityArguments(entity, tableInfo));

            object insertId = _dbCommandExecutor.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments));
            tableInfo.SetAutoNumber<TEntity>(entity, insertId);
        }
    }
}