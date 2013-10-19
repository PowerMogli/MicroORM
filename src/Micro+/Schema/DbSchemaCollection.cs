namespace MicroORM.Schema
{
    internal static class DbSchemaAllocator<TEntity>
    {
        private static DbSchemaReader SchemaReader { get; set; }

        static DbSchemaAllocator()
        {
            SchemaReader = DbSchemaAllocator.SchemaReader;
        }

        internal static void Allocate()
        {
            if (SchemaReader == null) return;

            SchemaReader.ReadSchema<TEntity>();
        }
    }

    internal static class DbSchemaAllocator
    {
        internal static DbSchemaReader SchemaReader { get; set; }
    }
}
