// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySet.cs" company="">
//   
// </copyright>
// <summary>
//   The entity set.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB
{
    using System.Collections.ObjectModel;

    using RabbitDB.Session;
    using RabbitDB.Query;

    /// <summary>
    /// The entity set.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class EntitySet<T> : Collection<T>
    {
        #region Methods

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="dbSession">
        /// The db session.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal EntitySet<T> Load(IBaseDbSession dbSession, IQuery query)
        {
            using (var reader = dbSession.GetEntityReader<T>(query))
            {
                while (reader.Read())
                {
                    Add(reader.Current);
                }
            }

            return this;
        }

        #endregion
    }
}