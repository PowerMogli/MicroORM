namespace MicroORM.Storage
{
    internal interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }
}
