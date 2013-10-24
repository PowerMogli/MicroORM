namespace RabbitDB.Query
{
    interface IArgumentQuery
    {
        QueryParameterCollection Arguments { get; }
        string SqlStatement { get; }
    }
}
