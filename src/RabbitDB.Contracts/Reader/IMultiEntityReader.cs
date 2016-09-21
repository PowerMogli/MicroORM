namespace RabbitDB.Contracts.Reader
{
    public interface IMultiEntityReader
    {
        #region Public Methods

        IEntitySet<TEntity> Read<TEntity>();

        #endregion
    }
}