using MicroORM.Storage;
using System;

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

        public abstract void ReadSchema<T>();

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
