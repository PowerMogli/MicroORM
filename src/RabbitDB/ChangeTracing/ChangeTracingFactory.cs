using RabbitDB.ChangeTracker;
using RabbitDB.Entity.ChangeTracker;
using RabbitDB.Materialization;
using RabbitDB.Utils;
using System;

namespace RabbitDB.ChangeTracing
{
    internal class ChangeTracingFactory
    {
        internal static IChangeTracer Create<TEntity>(TEntity entity)
        {
            Entity.Entity internalEntity = entity as Entity.Entity;
            if (internalEntity == null)
            {
                throw new ArgumentNullException("entity", "Entity can´t be null!");
            }

            IChangeTracer changeTracer = null;
            switch (internalEntity.ChangeTracerOption)
            {
                case ChangeTracerOption.Hashed:
                    changeTracer = CreateHashedTracer(entity);
                    break;
                case ChangeTracerOption.Notified:
                    changeTracer = CraeteNotifiedTracer(entity);
                    break;
                default:
                    throw new NotImplementedException("No default 'IChangeTracer' implementation available!");
            }

            return changeTracer;
        }

        private static IChangeTracer CreateHashedTracer<TEntity>(TEntity entity)
        {
            return new HashedChangeTracer(new EntityHashSetCreator<TEntity>(entity), new ValidEntityArgumentReader<TEntity>(entity));
        }

        private static IChangeTracer CraeteNotifiedTracer<TEntity>(TEntity entity)
        {
            var changeTracker = new Tracker();
            changeTracker.TrackObject(entity);

            return new NotifiedChangeTracer(changeTracker, new ValidEntityArgumentReader<TEntity>(entity));
        }
    }
}