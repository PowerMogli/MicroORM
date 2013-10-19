using System;
using MicroORM.Storage;

namespace MicroORM.Schema
{
    internal abstract class DbSchemaReader : IDisposable
    {
        protected IDbProvider DbProvider { get; set; }
        protected DbTableCollection Tables { get; private set; }

        internal DbSchemaReader()
        {
            this.Tables = new DbTableCollection();
        }

        public abstract DbTable ReadSchema<T>();

        public void Dispose()
        {
            this.DbProvider.Dispose();
        }

        public void Flush()
        {
            this.Tables.Clear();
        }
    }
}
