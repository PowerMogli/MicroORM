using MicroORM.Base.Storage;
using System.Data;

namespace MicroORM.Base.Query
{
    interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
