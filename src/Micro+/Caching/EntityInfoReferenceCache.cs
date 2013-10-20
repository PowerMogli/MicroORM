using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MicroORM.Entity;

namespace MicroORM.Caching
{
    internal class EntityInfoReferenceCache<TEntity>
    {
        private readonly object _lock = new object();
        private ConcurrentDictionary<Type, List<CacheItem<TEntity>>> _referenceCache;
        private List<Type> _keys;
        private Timer _cleanUpTimer;

        internal EntityInfoReferenceCache()
        {
            _referenceCache = new ConcurrentDictionary<Type, List<CacheItem<TEntity>>>();
            _keys = new List<Type>();
            _cleanUpTimer = new Timer(CleanUp, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(200));
        }

        internal EntityInfo Get(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            List<CacheItem<TEntity>> items;
            if (_referenceCache.TryGetValue(entity.GetType(), out items))
            {
                for (int index = 0; index < items.Count; index++)
                {
                    if (entity.Equals(items[index].Target))
                        return items[index].EntityInfo;
                }
            }
            return null;
        }

        internal void Add(TEntity entity, EntityInfo entityInfo)
        {
            List<CacheItem<TEntity>> items = new List<CacheItem<TEntity>>();
            if (_referenceCache.ContainsKey(entity.GetType()) == false)
            {
                _referenceCache.TryAdd(entity.GetType(), items);
                _keys.Add(entity.GetType());
                items.Add(new CacheItem<TEntity>(entity, entityInfo));
            }
        }

        internal void Update(TEntity entity, EntityInfo entityInfo)
        {
            List<CacheItem<TEntity>> items = new List<CacheItem<TEntity>>();
            if (_referenceCache.ContainsKey(entity.GetType()))
            {
                _referenceCache.TryGetValue(entity.GetType(), out items);
                for (int index = 0; index < items.Count; index++)
                {
                    if (entity.Equals(items[index].Target))
                    {
                        items[index].EntityInfo = entityInfo;
                        break;
                    }
                }
            }
        }

        private void CleanUp(object data)
        {
            if (_referenceCache.IsEmpty)
                return;

            List<Type> disposed = new List<Type>();

            for (int i = 0; i < _referenceCache.Count; i++)
            {
                Type key;
                lock (_lock) { key = _keys[i]; }

                List<CacheItem<TEntity>> items = _referenceCache[key];

                for (int j = 0; j < items.Count; j++)
                {
                    if (items[j].IsAlive == false || items[j].Target == null)
                        items.RemoveAt(j);
                }

                if (items.Count == 0)
                    disposed.Add(key);
            }
            foreach (Type key in disposed)
                Remove(key);
        }

        private void Remove(Type key)
        {
            lock (_lock) { _keys.Remove(key); }

            List<CacheItem<TEntity>> items;
            _referenceCache.TryRemove(key, out items);
        }
    }
}
