using System.Collections;
using System.Collections.Generic;
using RabbitDB.Query;

namespace RabbitDB.Base
{
    public class EntitySet<T> : IEnumerable<T>
    {
        private List<T> _list = new List<T>();

        internal EntitySet<T> Load(IDbSession dbSession, IQuery query)
        {
            using (EntityReader<T> reader = dbSession.GetEntityReader<T>(query))
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
