using System.Collections.ObjectModel;
using RabbitDB.Query;
using RabbitDB.Reader;

namespace RabbitDB.Base
{
    public class EntitySet<T> : Collection<T>
    {
        internal EntitySet<T> Load(IBaseDbSession dbSession, IQuery query)
        {
            using (EntityReader<T> reader = dbSession.GetEntityReader<T>(query))
            {
                while (reader.Read())
                {
                    base.Add(reader.Current);
                }
            }

            return this;
        }
    }
}