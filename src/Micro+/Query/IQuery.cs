using MicroORM.Storage;
using System.Data;

namespace MicroORM.Query
{
    interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
