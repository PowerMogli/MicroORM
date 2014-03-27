using RabbitDB.Base;
using RabbitDB.Materialization;
using RabbitDB.Storage;
using System.Data;

namespace RabbitDB.Reader
{
    public class MultiEntityReader
    {
        private IDataReader _dataReader;
        private IDbProvider _dbProvider;

        internal MultiEntityReader(IDataReader dataReader, IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
            _dataReader = dataReader;
        }

        public EntitySet<TEntity> Read<TEntity>()
        {
            EntityReader<TEntity> entityReader = new EntityReader<TEntity>(_dataReader, _dbProvider, new EntityMaterializer(_dbProvider));
            EntitySet<TEntity> entitySet = new EntitySet<TEntity>();

            while (entityReader.Read())
            {
                entitySet.Add(entityReader.Current);
            }

            return entitySet;
        }
    }
}