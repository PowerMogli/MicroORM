#region using directives

using System.Collections.Generic;

#endregion

namespace RabbitDB.Contracts
{
    public interface IEntitySet<out T> : IEnumerable<T>
    {
    }
}