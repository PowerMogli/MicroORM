// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbSession.cs" company="">
//   
// </copyright>
// <summary>
//   The DbSession interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq.Expressions;

using RabbitDB.Contracts.Entity;

#endregion

namespace RabbitDB.Contracts.Session
{
    /// <summary>
    ///     The DbSession interface.
    /// </summary>
    internal interface IDbSession : ITransactionalSession,
                                    IDisposable
    {
        #region Public Methods

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void Delete<TEntity>(TEntity entity);

        /// <summary>
        ///     The execute command.
        /// </summary>
        /// <param name="sql">
        ///     The sql.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        void ExecuteCommand(string sql, params object[] args);

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        void Insert<T>(T data);

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void Load<TEntity>(TEntity entity)
            where TEntity : IEntity;

        /// <summary>
        ///     The persist changes.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool PersistChanges<TEntity>(TEntity entity)
            where TEntity : IEntity;

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        /// <param name="setArguments">
        ///     The set arguments.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments);

        #endregion

        // void Update<T>(T data);
    }
}