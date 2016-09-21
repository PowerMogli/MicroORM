// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiEntityReader.cs" company="">
//   
// </copyright>
// <summary>
//   The multi entity reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts;
using RabbitDB.Contracts.Reader;
using RabbitDB.Materialization;

#endregion

namespace RabbitDB.Reader
{
    /// <summary>
    ///     The multi entity reader.
    /// </summary>
    public class MultiEntityReader : IMultiEntityReader
    {
        #region Fields

        /// <summary>
        ///     The _data reader.
        /// </summary>
        private readonly IDataReader _dataReader;

        /// <summary>
        ///     The _sql dialect.
        /// </summary>
        private readonly SqlDialect.SqlDialect _sqlDialect;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiEntityReader" /> class.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        internal MultiEntityReader(IDataReader dataReader, SqlDialect.SqlDialect sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _dataReader = dataReader;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The read.
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEntitySet<TEntity> Read<TEntity>()
        {
            EntityReader<TEntity> entityReader = new EntityReader<TEntity>(_dataReader, _sqlDialect.DbProvider, new EntityMaterializer(_sqlDialect));
            EntitySet<TEntity> entitySet = new EntitySet<TEntity>();

            while (entityReader.Read())
            {
                entitySet.Add(entityReader.Current);
            }

            return entitySet;
        }

        #endregion
    }
}