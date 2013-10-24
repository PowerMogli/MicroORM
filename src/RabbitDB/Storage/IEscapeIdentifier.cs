namespace RabbitDB.Storage
{
    internal interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }
}
