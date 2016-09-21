namespace RabbitDB.Contracts.Query.StoredProcedure
{
    public interface IStoredProcedure
    {
        #region  Properties

        IProcedureParameterCollection Parameters { get; }

        string StoredProcedureName { get; }

        #endregion
    }
}