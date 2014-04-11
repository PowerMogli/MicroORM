// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBaseDbSession.cs" company="">
//   
// </copyright>
// <summary>
//   The BaseDbSession interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using System;

    using RabbitDB.Query;
    using RabbitDB.Reader;

    /// <summary>
    /// The BaseDbSession interface.
    /// </summary>
    internal interface IBaseDbSession : ITransactionalSession, IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        EntityReader<T> GetEntityReader<T>(IQuery query);

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        EntitySet<T> GetEntitySet<T>(IQuery query);

        #endregion
    }
}