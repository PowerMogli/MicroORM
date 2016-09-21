// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySet.cs" company="">
//   
// </copyright>
// <summary>
//   The entity set.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using RabbitDB.Contracts;
using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Session;

#endregion

namespace RabbitDB
{
    /// <summary>
    ///     The entity set.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class EntitySet<T> : IEntitySet<T>
    {
        #region Fields

        private readonly Collection<T> _collection;

        #endregion

        #region Construction

        internal EntitySet()
        {
            _collection = new Collection<T>();
        }

        #endregion

        #region Public Methods

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region Internal Methods

        internal void Add(T entity)
        {
            _collection.Add(entity);
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="dbSession">
        ///     The db session.
        /// </param>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal IEntitySet<T> Load(IBaseDbSession dbSession, IQuery query)
        {
            using (IEntityReader<T> reader = dbSession.GetEntityReader<T>(query))
            {
                while (reader.Read())
                {
                    _collection.Add(reader.Current);
                }
            }

            return this;
        }

        #endregion

        #region Private Methods

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion
    }
}