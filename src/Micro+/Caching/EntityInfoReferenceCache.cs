using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using MicroORM.Entity;

namespace MicroORM.Caching
{
    internal class EntityInfoReferenceCache<TEntity> : IDisposable
    {
        private readonly object _lock = new object();
        private ConcurrentDictionary<Type, List<CacheItem<TEntity>>> _referenceCache;
        private List<Type> _keys;
        private BackgroundWorker _cleanUpWorker;
        private TimeSpan TIMEOUT = TimeSpan.FromMilliseconds(100);
        private const byte ROUNDS_FOR_GC = 100;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        internal EntityInfoReferenceCache()
        {
            _referenceCache = new ConcurrentDictionary<Type, List<CacheItem<TEntity>>>();
            _keys = new List<Type>();
            _cleanUpWorker = new BackgroundWorker();
            _cleanUpWorker.DoWork += new DoWorkEventHandler(StartCleanUp);
            _cleanUpWorker.WorkerSupportsCancellation = true;
            _cleanUpWorker.RunWorkerAsync();
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
            List<CacheItem<TEntity>> items;

            if (_referenceCache.TryGetValue(entity.GetType(), out items) == false)
            {
                items = new List<CacheItem<TEntity>>();
                _referenceCache.TryAdd(entity.GetType(), items);
            }
            if (_keys.Contains(entity.GetType()) == false)
                _keys.Add(entity.GetType());
            items.Add(new CacheItem<TEntity>(entity, entityInfo));
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

        private void StartCleanUp(object sender, DoWorkEventArgs args)
        {
            byte roundCount = 0;
            while (_cleanUpWorker.CancellationPending == false)
            {
                Thread.Sleep(TIMEOUT);
                if (roundCount++ > ROUNDS_FOR_GC)
                {
                    CleanUp();
                    roundCount = 0;
                }
            }
            _waitHandle.Set();
        }

        private void CleanUp()
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

        public void Dispose()
        {
            _cleanUpWorker.CancelAsync();
            _waitHandle.WaitOne();
            _cleanUpWorker.Dispose();
            _cleanUpWorker = null;
            _keys.Clear();
            _referenceCache.Clear();
        }
    }
}
