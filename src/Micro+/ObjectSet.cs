using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MicroORM.Query;

namespace MicroORM.Base
{
    public class ObjectSet<T> : IEnumerable<T>
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

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
