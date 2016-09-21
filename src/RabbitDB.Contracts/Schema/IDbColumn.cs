namespace RabbitDB.Contracts.Schema
{
    public interface IDbColumn
    {
        #region  Properties

        string DefaultValue { get; set; }

        bool IsNullable { get; set; }

        bool IsPrimaryKey { get; set; }

        string Name { get; set; }

        #endregion

        #region Public Methods

        bool IsToSkip(string resolvedColumnName);

        #endregion
    }
}