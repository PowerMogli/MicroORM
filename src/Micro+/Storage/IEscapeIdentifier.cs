namespace MicroORM.Storage
{
    public interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }
}
