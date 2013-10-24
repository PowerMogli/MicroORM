using System.Collections;
using System.Collections.Generic;
using MicroORM.Query;

namespace MicroORM.Base
{
    public class ObjectSet<T> : IEnumerable<T>
    {
        private List<T> _list = new List<T>();

        internal ObjectSet<T> Load(IDbSession dbSession, IQuery query)
        {
            using (ObjectReader<T> reader = dbSession.GetObjectReader<T>(query))
            {
                while (reader.Read())
                {
                    _list.Add(reader.Current);
                }
            }

            return this;
        }

        public T this[int index]
        {
            get { return _list[index]; }
        }

        public int Count { get { return _list.Count; } }
        
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
