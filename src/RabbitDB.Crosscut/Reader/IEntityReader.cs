
namespace RabbitDB.Crosscut.Reader
{
    internal interface IEntityReader<TEntity>
    {
        TEntity Current { get; }
        bool Read();
    }
}