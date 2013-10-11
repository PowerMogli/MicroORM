using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;
using MicroORM.Query.Generic;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class EntityQuery<T> : SqlQuery<T>, IQuery where T : Entity
    {
        internal EntityQuery(T entity)
            : base(null)
        {
            this.Entity = entity;
        }

        internal T Entity { get; set; }

        public new IDbCommand Compile(IDbProvider provider)
        {
            TableInfo tableInfo = TableInfo.GetTableInfo(this.Entity.GetType());
            base._primaryKey = tableInfo.GetPrimaryKey(this.Entity);

            return base.Compile(provider);
        }
    }
}
