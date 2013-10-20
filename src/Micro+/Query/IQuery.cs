using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
