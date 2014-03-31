using RabbitDB.Base;
using RabbitDB.Materialization;
using System.Data;

namespace RabbitDB.Reader
{
    public class MultiEntityReader
    {
        private IDataReader _dataReader;
        private SqlDialect.SqlDialect _sqlDialect;

        internal MultiEntityReader(IDataReader dataReader, SqlDialect.SqlDialect sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _dataReader = dataReader;
        }

        public EntitySet<TEntity> Read<TEntity>()
        {
            EntityReader<TEntity> entityReader = new EntityReader<TEntity>(_dataReader, _sqlDialect.DbProvider, new EntityMaterializer(_sqlDialect));
            EntitySet<TEntity> entitySet = new EntitySet<TEntity>();

            while (entityReader.Read())
            {
                entitySet.Add(entityReader.Current);
            }

            return entitySet;
        }
    }
}