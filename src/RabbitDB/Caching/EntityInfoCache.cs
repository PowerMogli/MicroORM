using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using RabbitDB.Entity;

namespace RabbitDB.Caching
{
    internal class EntityInfoCache<TEntity> : IDisposable
    {
        private readonly object _lock = new object();
        private ConcurrentDictionary<int, CacheItem<TEntity>> _referenceCache;
        private List<int> _keys;
        private BackgroundWorker _cleanUpWorker;
        private TimeSpan TIMEOUT = TimeSpan.FromMilliseconds(100);
        private const byte ROUNDS_FOR_GC = 20;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        internal EntityInfoCache()
        {
            _referenceCache = new ConcurrentDictionary<int, CacheItem<TEntity>>();
            _keys = new List<int>();
            _cleanUpWorker = new BackgroundWorker();
            _cleanUpWorker.DoWork += new DoWorkEventHandler(StartCleanUp);
            _cleanUpWorker.WorkerSupportsCancellation = true;
            _cleanUpWorker.RunWorkerAsync();
        }

        internal EntityInfo Get(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            CacheItem<TEntity> item;
            if (_referenceCache.TryGetValue(entity.GetHashCode(), out item))
                return item.EntityInfo;

            return null;
        }

        internal void Add(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> item;
            int hash = entity.GetHashCode();

            if (_referenceCache.TryGetValue(hash, out item) == false)
            {
                item = new CacheItem<TEntity>(entity, entityInfo);
                _referenceCache.TryAdd(hash, item);
            }
            lock (_lock)
            {
                if (_keys.Contains(hash) == false)
                    _keys.Add(hash);
            }
        }

        internal void Update(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> item;
            if (_referenceCache.TryGetValue(entity.GetHashCode(), out item))
                item.EntityInfo = entityInfo;
        }

        internal void Remove(TEntity entity)
        {
            CacheItem<TEntity> cacheItem;
            int entityHash = entity.GetHashCode();
            if (_referenceCache.TryRemove(entityHash, out cacheItem))
            {
                lock (_lock)
                {
                    _keys.Remove(entityHash);
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
            if (_referenceCache.IsEmpty) return;

            List<int> disposed = new List<int>();

            for (int i = 0; i < _referenceCache.Count; i++)
            {
                int key;
                if (_keys.Count - 1 < i) return;

                lock (_lock) { key = _keys[i]; }

                CacheItem<TEntity> item = _referenceCache[key];
                if (item.IsAlive == false
                    || item.Target == null
                    || item.EntityInfo.CanBeRemoved)
                    disposed.Add(key);
            }
            foreach (int key in disposed)
                Remove(key);
        }

        private void Remove(int key)
        {
            lock (_lock) { _keys.Remove(key); }

            CacheItem<TEntity> item;
            _referenceCache.TryRemove(key, out item);
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
