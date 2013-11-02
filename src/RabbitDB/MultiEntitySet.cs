using RabbitDB.Reader;

namespace RabbitDB.Base
{
    public class MultiEntitySet
    {
        private MultiEntityReader _reader;

        internal MultiEntitySet(MultiEntityReader reader)
        {
            _reader = reader;
        }

        public EntitySet<TEntity> Read<TEntity>()
        {
            return _reader.Read<TEntity>();
        }
    }
}