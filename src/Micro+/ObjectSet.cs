using System.Collections.Generic;
using MicroORM.Query;
using System.Linq;

namespace MicroORM.Base
{
    public class ObjectSet<T>
    {
        private List<T> _list = new List<T>();

        internal ObjectSet<T> Load(IDbSession dbSession, IQuery query)
        {
            ObjectReader<T> reader = dbSession.GetObjectReader<T>(query);
            while (reader.Read())
            {
                _list.Add(reader.Current);
            }

            return this;
        }

        internal T SingleOrDefault()
        {
            return _list.SingleOrDefault();
        }
    }
}
