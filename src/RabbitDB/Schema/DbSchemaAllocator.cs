namespace RabbitDB.Schema
{
    internal static class DbSchemaAllocator<TEntity>
    {
        private static DbSchemaReader SchemaReader { get; set; }
        private static DbTable DbInternTable { get; set; }

        static DbSchemaAllocator()
        {
            SchemaReader = DbSchemaAllocator.SchemaReader;
        }

        internal static DbTable DbTable
        {
            get
            {
                if (SchemaReader == null) return null;
                if (DbInternTable != null) return DbInternTable;

                return DbInternTable = SchemaReader.ReadSchema<TEntity>();
            }
        }
    }

    public static class DbSchemaAllocator
    {
        internal static DbSchemaReader SchemaReader { get; set; }

        /// <summary>
        /// Flushes information for all tables.
        /// </summary>
        public static void Flush()
        {
            SchemaReader.Flush();
        }

        /// <summary>
        /// Flushes information for one specific table.
        /// </summary>
        /// <param name="tableName">The name of the table to flush the information for.</param>
        public static void Flush(string tableName)
        {
            SchemaReader.Flush(tableName);
        }
    }
}
