namespace MicroORM.Schema
{
    internal static class DbSchemaAllocator<TEntity>
    {
        private static DbSchemaReader SchemaReader { get; set; }

        static DbSchemaAllocator()
        {
            SchemaReader = DbSchemaAllocator.SchemaReader;
        }

        internal static DbTable DbTable
        {
            get
            {
                if (SchemaReader == null) return null;

                return SchemaReader.ReadSchema<TEntity>();
            }
        }
    }

    public static class DbSchemaAllocator
    {
        internal static DbSchemaReader SchemaReader { get; set; }

        public static void FlushReader()
        {
            SchemaReader.Flush();
        }
    }
}
