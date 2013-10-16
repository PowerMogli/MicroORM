using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    public interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
