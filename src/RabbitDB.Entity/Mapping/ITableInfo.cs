using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitDB.Mapping
{
    public interface ITableInfo
    {
        string ResolveColumnName(string key);
    }
}
