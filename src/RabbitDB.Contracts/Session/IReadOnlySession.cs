// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlySession.cs" company="">
//   
// </copyright>
// <summary>
//   The ReadOnlySession interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;

using RabbitDB.Contracts.Reader;

namespace RabbitDB.Contracts.Session
{
    /// <summary>
    /// The ReadOnlySession interface.
    /// </summary>
    internal interface IReadOnlySession : ITransactionalSession, IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Executes the multiple.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        IMultiEntityReader ExecuteMultiple(string sql, params object[] arguments);

        /// <summary>
        /// The get column value.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="V">
        /// </typeparam>
        /// <returns>
        /// The <see cref="V"/>.
        /// </returns>
        V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria);

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetEntity<T>(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <param name="additionalPredicate">
        /// The additional predicate.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetEntity<T>(object primaryKey, string additionalPredicate = null);

        /// <summary>
        /// The get entity.
        /// </summary>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <param name="additionalPredicate">
        /// The additional predicate.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetEntity<T>(object[] primaryKey, string additionalPredicate = null);

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntityReader<T> GetEntityReader<T>();

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntityReader<T> GetEntityReader<T>(Expression<Func<T, bool>> condition);

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntityReader<T> GetEntityReader<T>(string sql, params object[] args);

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntitySet<T> GetEntitySet<T>();

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntitySet<T> GetEntitySet<T>(Expression<Func<T, bool>> condition);

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntitySet<T> GetEntitySet<T>(string sql, params object[] args);

        /// <summary>
        /// The get scalar value.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetScalarValue<T>(string sql, params object[] args);

        #endregion
    }
}