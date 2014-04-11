// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSchemaAllocator.cs" company="">
//   
// </copyright>
// <summary>
//   The db schema allocator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    /// <summary>
    /// The db schema allocator.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal static class DbSchemaAllocator<TEntity>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="DbSchemaAllocator"/> class.
        /// </summary>
        static DbSchemaAllocator()
        {
            SchemaReader = DbSchemaAllocator.SchemaReader;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the db table.
        /// </summary>
        internal static DbTable DbTable
        {
            get
            {
                if (SchemaReader == null)
                {
                    return null;
                }

                if (DbInternTable != null)
                {
                    return DbInternTable;
                }

                return DbInternTable = SchemaReader.ReadSchema<TEntity>();
            }
        }

        /// <summary>
        /// Gets or sets the db intern table.
        /// </summary>
        private static DbTable DbInternTable { get; set; }

        /// <summary>
        /// Gets or sets the schema reader.
        /// </summary>
        private static DbSchemaReader SchemaReader { get; set; }

        #endregion
    }

    /// <summary>
    /// The db schema allocator.
    /// </summary>
    public static class DbSchemaAllocator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the schema reader.
        /// </summary>
        internal static DbSchemaReader SchemaReader { get; set; }

        #endregion
    }
}