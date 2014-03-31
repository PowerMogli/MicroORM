using System;

namespace RabbitDB.Storage
{
    internal interface INullValueResolver
    {
        object ResolveNullValue(object value, Type type);
    }
}