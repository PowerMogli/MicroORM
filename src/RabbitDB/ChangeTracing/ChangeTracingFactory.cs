// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTracingFactory.cs" company="">
//   
// </copyright>
// <summary>
//   The change tracing factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.ChangeTracing
{
    using System;

    using RabbitDB.ChangeTracker;
    using RabbitDB.Entity;
    using RabbitDB.Entity.ChangeRecorder;
    using RabbitDB.Materialization;
    using RabbitDB.Utils;

    /// <summary>
    /// The change tracing factory.
    /// </summary>
    internal class ChangeTracingFactory
    {
        #region Methods

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IChangeRecorder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal static IChangeRecorder Create<TEntity>(TEntity entity)
        {
            var internalEntity = entity as Entity;
            if (internalEntity == null)
            {
                throw new ArgumentNullException("entity", "Entity can´t be null!");
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

        /// <summary>
        /// The craete notified tracer.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IChangeRecorder"/>.
        /// </returns>
        private static IChangeRecorder CraeteNotifiedTracer<TEntity>(TEntity entity)
        {
            var changeTracker = new Tracker();
            changeTracker.TrackObject(entity);

            return new NotifiedChangeRecorder(changeTracker, new ValidEntityArgumentReader<TEntity>(entity));
        }

        /// <summary>
        /// The create hashed tracer.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IChangeRecorder"/>.
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