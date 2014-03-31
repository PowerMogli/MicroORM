using RabbitDB.ChangeTracker;
using RabbitDB.Entity.ChangeRecorder;
using RabbitDB.Materialization;
using RabbitDB.Utils;
using System;

namespace RabbitDB.ChangeTracing
{
    internal class ChangeTracingFactory
    {
        internal static IChangeRecorder Create<TEntity>(TEntity entity)
        {
            Entity.Entity internalEntity = entity as Entity.Entity;
            if (internalEntity == null)
            {
                throw new ArgumentNullException("entity", "Entity can´t be null!");
            }

            IChangeRecorder changeTracer = null;
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

        private static IChangeRecorder CreateHashedTracer<TEntity>(TEntity entity)
        {
            return new HashedChangeRecorder(new EntityHashSetCreator<TEntity>(entity), new ValidEntityArgumentReader<TEntity>(entity));
        }

        private static IChangeRecorder CraeteNotifiedTracer<TEntity>(TEntity entity)
        {
            var changeTracker = new Tracker();
            changeTracker.TrackObject(entity);

            return new NotifiedChangeRecorder(changeTracker, new ValidEntityArgumentReader<TEntity>(entity));
        }
    }
}