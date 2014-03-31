using RabbitDB.Materialization;
using RabbitDB.Reader;
using System.Data;

namespace RabbitDB.Storage
{
    internal class DbCommandExecutor : IDbCommandExecutor
    {
        private IDbProvider _dbProvider;

        public INullValueResolver NullValueResolver { get; set; }

        internal DbCommandExecutor(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        internal DbCommandExecutor(IDbProvider dbProvider, INullValueResolver nullValueResolver)
            : this(dbProvider)
        {
            this.NullValueResolver = nullValueResolver;
        }

        public void ExecuteCommand(IDbCommand dbCommand)
        {
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            finally
            {
                _dbProvider.Dispose();
            }
        }

        public IDataReader ExecuteReader(IDbCommand dbCommand)
        {
            return dbCommand.ExecuteReader();
        }

        public virtual EntityReader<T> ExecuteReader<T>(IDbCommand dbCommand)
        {
            IDataReader dataReader = dbCommand.ExecuteReader();
            return new EntityReader<T>(dataReader, _dbProvider, new EntityMaterializer(this.NullValueResolver));
        }

        public T ExecuteScalar<T>(IDbCommand dbCommand)
        {
            try
            {
                return (T)dbCommand.ExecuteScalar();
            }
            finally
            {
                _dbProvider.Dispose();
            }
        }
    }
}