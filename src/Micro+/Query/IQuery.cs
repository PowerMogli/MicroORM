using MicroORM.Storage;
using System.Data;

namespace MicroORM.Query
{
    public interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
