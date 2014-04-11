// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfoCache.cs" company="">
//   
// </copyright>
// <summary>
//   The entity info cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using RabbitDB.Entity;

    /// <summary>
    /// The entity info cache.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class EntityInfoCache<TEntity> : IDisposable
    {
        #region Constants

        /// <summary>
        /// The round s_ fo r_ gc.
        /// </summary>
        private const byte RoundsForGc = 20;

        #endregion

        #region Fields

        /// <summary>
        /// The _keys.
        /// </summary>
        private readonly List<int> _keys;

        /// <summary>
        /// The _lock.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The _reference cache.
        /// </summary>
        private readonly ConcurrentDictionary<int, CacheItem<TEntity>> _referenceCache;

        /// <summary>
        /// The timeout.
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// The _wait handle.
        /// </summary>
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

        /// <summary>
        /// The _clean up worker.
        /// </summary>
        private BackgroundWorker _cleanUpWorker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfoCache{TEntity}"/> class.
        /// </summary>
        internal EntityInfoCache()
        {
            _referenceCache = new ConcurrentDictionary<int, CacheItem<TEntity>>();
            _keys = new List<int>();
            _cleanUpWorker = new BackgroundWorker();
            _cleanUpWorker.DoWork += this.StartCleanUp;
            _cleanUpWorker.WorkerSupportsCancellation = true;
            _cleanUpWorker.RunWorkerAsync();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            _cleanUpWorker.CancelAsync();
            _waitHandle.WaitOne();
            _cleanUpWorker.Dispose();
            _cleanUpWorker = null;

            foreach (var key in _keys)
            {
                Remove(key);
            }

            _keys.Clear();
            _referenceCache.Clear();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entityInfo">
        /// The entity info.
        /// </param>
        internal void Add(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> cacheItem;
            var hash = entity.GetHashCode();

            if (_referenceCache.TryGetValue(hash, out cacheItem) == false)
            {
                cacheItem = new CacheItem<TEntity>(entity, entityInfo);
                _referenceCache.TryAdd(hash, cacheItem);
            }

            lock (this._lock)
            {
                if (_keys.Contains(hash) == false)
                {
                    _keys.Add(hash);
                }
            }
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EntityInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal EntityInfo Get(TEntity entity)
        {
            if (entity.Equals(null))
            {
                throw new ArgumentNullException("entity");
            }

            CacheItem<TEntity> item;

            return _referenceCache.TryGetValue(entity.GetHashCode(), out item) ? item.EntityInfo : null;
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal void Remove(TEntity entity)
        {
            CacheItem<TEntity> cacheItem;
            var entityHash = entity.GetHashCode();
            if (!_referenceCache.TryRemove(entityHash, out cacheItem))
            {
                return;
            }

            lock (this._lock)
            {
                _keys.Remove(entityHash);
            }

            var target = cacheItem.Target as IDisposable;
            if (target != null)
            {
                target.Dispose();
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entityInfo">
        /// The entity info.
        /// </param>
        internal void Update(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> item;
            if (_referenceCache.TryGetValue(entity.GetHashCode(), out item))
            {
                item.EntityInfo = entityInfo;
            }
        }

        /// <summary>
        /// The clean up.
        /// </summary>
        private void CleanUp()
        {
            if (_referenceCache.IsEmpty)
            {
                return;
            }

            for (var i = 0; i < this._referenceCache.Count; i++)
            {
                int key;
                if (_keys.Count - 1 < i)
                {
                    return;
                }

                lock (_lock)
                {
                    key = _keys[i];
                }

                CacheItem<TEntity> item = _referenceCache[key];
                if (item.IsAlive == false
                    || item.Target.Equals(null)
                    || item.EntityInfo.CanBeRemoved)
                {
                    Remove(key);
                }
            }
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        private void Remove(int key)
        {
            lock (_lock)
            {
                _keys.Remove(key);
            }

            CacheItem<TEntity> item;
            _referenceCache.TryRemove(key, out item);
            item.EntityInfo.Dispose();
        }

        /// <summary>
        /// The start clean up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void StartCleanUp(object sender, DoWorkEventArgs args)
        {
            byte roundCount = 0;
            while (_cleanUpWorker.CancellationPending == false)
            {
                Thread.Sleep(_timeout);
                if (roundCount++ <= RoundsForGc)
                {
                    continue;
                }

                CleanUp();
                roundCount = 0;
            }

            _waitHandle.Set();
        }

        #endregion
    }
}