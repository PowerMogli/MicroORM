// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiEntityReader.cs" company="">
//   
// </copyright>
// <summary>
//   The multi entity reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

using RabbitDB.Materialization;

namespace RabbitDB.Reader
{
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The multi entity reader.
    /// </summary>
    public class MultiEntityReader
    {
        /// <summary>
        /// The _data reader.
        /// </summary>
        private readonly IDataReader _dataReader;

        /// <summary>
        /// The _sql dialect.
        /// </summary>
        private readonly SqlDialect _sqlDialect;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiEntityReader"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal MultiEntityReader(IDataReader dataReader, SqlDialect sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _dataReader = dataReader;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntitySet<TEntity> Read<TEntity>()
        {
            var entityReader = new EntityReader<TEntity>(_dataReader, _sqlDialect.DbProvider, new EntityMaterializer(_sqlDialect));
            var entitySet = new EntitySet<TEntity>();

            while (entityReader.Read())
            {
                entitySet.Add(entityReader.Current);
            }

            return entitySet;
        }
    }
}