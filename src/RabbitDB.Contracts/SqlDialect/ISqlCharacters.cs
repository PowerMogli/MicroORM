namespace RabbitDB.Contracts.SqlDialect
{
    internal interface ISqlCharacters
    {
        #region  Properties

        string ParameterPrefix { get; }

        #endregion

        #region Public Methods

        string EscapeName(string value);

        #endregion
    }
}