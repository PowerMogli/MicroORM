using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Utils;

namespace RabbitDB.Storage
{
    internal class DbPersister : IDbPersister
    {
        private IDbProvider _dbProvider;

        internal DbPersister(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public void Update<TEntity>(IQuery query)
        {
            _dbProvider.ExecuteCommand(query);
        }

        public void Delete<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string deleteStatement = tableInfo.CreateDeleteStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(tableInfo.GetPrimaryKeyValues(entity));

            _dbProvider.ExecuteCommand(new SqlQuery(deleteStatement, arguments));
        }

        public void Insert<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string insertStatement = tableInfo.CreateInsertStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(new EntityArgumentsReader().GetEntityArguments(entity, tableInfo));

            object insertId = _dbProvider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments));
            tableInfo.SetAutoNumber<TEntity>(entity, insertId);
        }
    }
}