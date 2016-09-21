// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfoCache.cs" company="">
//   
// </copyright>
// <summary>
//   The entity info cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using RabbitDB.Entity;
using RabbitDB.Entity.Entity;

#endregion

namespace RabbitDB.Caching
{
    /// <summary>
    ///     The entity info cache.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class EntityInfoCache<TEntity> : IDisposable
    {
        #region Fields

        /// <summary>
        ///     The round s_ fo r_ gc.
        /// </summary>
        private const byte RoundsForGc = 20;

        /// <summary>
        ///     The _keys.
        /// </summary>
        private readonly List<int> _keys;

        /// <summary>
        ///     The _lock.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        ///     The _reference cache.
        /// </summary>
        private readonly ConcurrentDictionary<int, CacheItem<TEntity>> _referenceCache;

        /// <summary>
        ///     The timeout.
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);

        /// <summary>
        ///     The _wait handle.
        /// </summary>
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

        /// <summary>
        ///     The _clean up worker.
        /// </summary>
        private BackgroundWorker _cleanUpWorker;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityInfoCache{TEntity}" /> class.
        /// </summary>
        internal EntityInfoCache()
        {
            _referenceCache = new ConcurrentDictionary<int, CacheItem<TEntity>>();
            _keys = new List<int>();
            _cleanUpWorker = new BackgroundWorker();
            _cleanUpWorker.DoWork += StartCleanUp;
            _cleanUpWorker.WorkerSupportsCancellation = true;
            _cleanUpWorker.RunWorkerAsync();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            _cleanUpWorker.CancelAsync();
            _waitHandle.WaitOne();
            _cleanUpWorker.Dispose();
            _cleanUpWorker = null;

            foreach (int key in _keys)
            {
                Remove(key);
            }

            _keys.Clear();
            _referenceCache.Clear();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="entityInfo">
        ///     The entity info.
        /// </param>
        internal void Add(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> cacheItem;
            int hash = entity.GetHashCode();

            if (_referenceCache.TryGetValue(hash, out cacheItem) == false)
            {
                cacheItem = new CacheItem<TEntity>(entity, entityInfo);
                _referenceCache.TryAdd(hash, cacheItem);
            }

            lock (_lock)
            {
                if (_keys.Contains(hash) == false)
                {
                    _keys.Add(hash);
                }
            }
        }

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="EntityInfo" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal EntityInfo Get(TEntity entity)
        {
            if (entity.Equals(null))
            {
                throw new ArgumentNullException(nameof(entity));
            }

            CacheItem<TEntity> item;

            return _referenceCache.TryGetValue(entity.GetHashCode(), out item)
                ? item.EntityInfo
                : null;
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        internal void Remove(TEntity entity)
        {
            CacheItem<TEntity> cacheItem;
            int entityHash = entity.GetHashCode();
            if (!_referenceCache.TryRemove(entityHash, out cacheItem))
            {
                return;
            }

            lock (_lock)
            {
                _keys.Remove(entityHash);
            }

            IDisposable target = cacheItem.Target as IDisposable;

            target?.Dispose();
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="entityInfo">
        ///     The entity info.
        /// </param>
        internal void Update(TEntity entity, EntityInfo entityInfo)
        {
            CacheItem<TEntity> item;
            if (_referenceCache.TryGetValue(entity.GetHashCode(), out item))
            {
                item.EntityInfo = entityInfo;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The clean up.
        /// </summary>
        private void CleanUp()
        {
            if (_referenceCache.IsEmpty)
            {
                return;
            }

            for (int i = 0; i < _referenceCache.Count; i++)
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
        ///     The remove.
        /// </summary>
        /// <param name="key">
        ///     The key.
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
        ///     The start clean up.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
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