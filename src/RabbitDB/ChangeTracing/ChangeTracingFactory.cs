// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTracingFactory.cs" company="">
//   
// </copyright>
// <summary>
//   The change tracing factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Entity.ChangeRecorder;
using RabbitDB.Entity.ChangeTracker;
using RabbitDB.Materialization;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.ChangeTracing
{
    /// <summary>
    ///     The change tracing factory.
    /// </summary>
    internal class ChangeTracingFactory
    {
        #region Internal Methods

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IChangeRecorder" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal static IChangeRecorder Create<TEntity>(TEntity entity)
        {
            Entity.Entity.Entity internalEntity = entity as Entity.Entity.Entity;
            if (internalEntity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity can´t be null!");
            }

            IChangeRecorder changeTracer;

            switch (internalEntity.ChangeTracerOption)
            {
                case ChangeRecorderOption.Hashed:
                    changeTracer = CreateHashedTracer(entity);
                    break;
                case ChangeRecorderOption.Notified:
                    changeTracer = CraeteNotifiedTracer(entity);
                    break;
                default:
                    throw new NotImplementedException("No default 'IChangeTracer' implementation available!");
            }

            return changeTracer;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The craete notified tracer.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IChangeRecorder" />.
        /// </returns>
        private static IChangeRecorder CraeteNotifiedTracer<TEntity>(TEntity entity)
        {
            Tracker changeTracker = new Tracker();
            changeTracker.TrackObject(entity);

            return new NotifiedChangeRecorder(changeTracker, new ValidEntityArgumentReader<TEntity>(entity));
        }

        /// <summary>
        ///     The create hashed tracer.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IChangeRecorder" />.
        /// </returns>
        private static IChangeRecorder CreateHashedTracer<TEntity>(TEntity entity)
        {
            return new HashedChangeRecorder(
                new EntityHashSetCreator<TEntity>(entity),
                new ValidEntityArgumentReader<TEntity>(entity));
        }

        #endregion
    }
}