using System.Data;
using RabbitDB.Base;
using RabbitDB.Storage;

namespace RabbitDB.Reader
{
    internal class MultiEntityReader
    {
        private IDataReader _dataReader;
        private IDbProvider _dbProvider;

        internal MultiEntityReader(IDataReader dataReader, IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
            _dataReader = dataReader;
        }

        internal EntitySet<TEntity> Read<TEntity>()
        {
            EntityReader<TEntity> entityReader = new EntityReader<TEntity>(_dataReader, _dbProvider);
            EntitySet<TEntity> entitySet = new EntitySet<TEntity>();

            while (entityReader.Read())
            {
                entitySet.Add(entityReader.Current);
            }

            return entitySet;
        }
    }
}