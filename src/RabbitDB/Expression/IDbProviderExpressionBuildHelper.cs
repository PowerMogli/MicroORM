using RabbitDB.Storage;

namespace RabbitDB.Expressions
{
    internal interface IDbProviderExpressionBuildHelper : IEscapeDbIdentifier
    {
        string FormatBoolean(bool value);
        string Substring(string column, int pos, int length);
        string ToUpper(string column);
        string ToLower(string column);
        string Length(string column);
    }
}
