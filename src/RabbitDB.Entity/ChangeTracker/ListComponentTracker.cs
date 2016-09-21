#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using RabbitDB.ChangeTracker;

#endregion

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.Entity.ChangeTracker
{
    /// <summary>
    ///     ListComponentTracker is used to track IEnumerable objects
    /// </summary>
    internal class ListComponentTracker : IComponentTracker
    {
        #region Fields

        private Dictionary<object, IComponentTracker> _componentTrackers;
        private bool _disposed;
        private ComponentTrackerHelper _helper;
        private bool _isDirty;
        private IEnumerable _objectToTrack;
        private TrackerInfo _trackerInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Default Constructor
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        public ListComponentTracker(ComponentTrackerHelper helper, IEnumerable objectToTrack, TrackerInfo trackerInfo)
        {
            Initialize(helper, objectToTrack, trackerInfo);
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Event that is raised when the is dirty flag is changed
        /// </summary>
        public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

        /// <summary>
        ///     True if the component or one of it's children is dirty
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty || _componentTrackers.Values.Any(x => x.IsDirty); }
        }

        /// <summary>
        ///     The component being tracker
        /// </summary>
        public object TrackedComponent => _objectToTrack;

        #endregion

        #region Public Methods

        /// <summary>
        ///     A list of child trackers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComponentTracker> ChildTrackers()
        {
            return _componentTrackers.Values;
        }

        /// <summary>
        ///     Dispose the tracker
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (IComponentTracker componentTracker in _componentTrackers.Values)
            {
                componentTracker.Dispose();
            }
            _componentTrackers = null;
            _disposed = true;
        }

        /// <summary>
        ///     mark the component and all it's children as clean
        /// </summary>
        public void MarkAsClean()
        {
            _isDirty = false;

            foreach (IComponentTracker componentTracker in _componentTrackers.Values)
            {
                componentTracker.MarkAsClean();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Add children to be tracked
        /// </summary>
        /// <param name="childObjects"></param>
        /// <returns></returns>
        private bool AddChildren(IEnumerable childObjects)
        {
            bool returnValue = false;
            IListComponentFilter listFilter = _trackerInfo.ListFilter;

            // if we have child tracker info create a new tracker for the object
            if (_trackerInfo.ChildTrackerInfo != null)
            {
                foreach (object child in childObjects)
                {
                    // filter the object if a filter exists
                    if (listFilter != null && !listFilter.FilterComponent(child))
                    {
                        continue;
                    }

                    IComponentTracker childTracker =
                        _helper.CreateTracker(child, _trackerInfo.ChildTrackerInfo);

                    childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

                    _componentTrackers.Add(child, childTracker);

                    returnValue = true;
                }
            }
            else
            {
                // Create a leaf tracker for each child in the list
                foreach (object child in childObjects)
                {
                    if (listFilter != null && !listFilter.FilterComponent(child))
                    {
                        continue;
                    }

                    IComponentTracker childTracker =
                        _helper.CreateTracker(child, new TrackerInfo
                        {
                            TrackerType = ComponentTrackerType.Leaf
                        });

                    childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

                    _componentTrackers.Add(child, childTracker);

                    returnValue = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        ///     Handler for child tracker is dirty changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ChildTrackerIsDirtyChanged(object sender, IsDiryChangedArgs eventArgs)
        {
            // short circuit if we are already dirty
            if (_isDirty)
            {
                return;
            }

            IComponentTracker childTracker = (IComponentTracker)sender;
            bool newValue = childTracker.IsDirty;
            bool fireChange = true;

            foreach (KeyValuePair<object, IComponentTracker> kvp in _componentTrackers)
            {
                if (kvp.Key == sender
                    || kvp.Value.IsDirty == newValue)
                {
                    continue;
                }

                fireChange = false;
                break;
            }

            if (fireChange)
            {
                IsDirtyChanged?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        ///     Initialize the tracker
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        private void Initialize(ComponentTrackerHelper helper, IEnumerable objectToTrack, TrackerInfo trackerInfo)
        {
            _componentTrackers = new Dictionary<object, IComponentTracker>();

            _helper = helper;
            _objectToTrack = objectToTrack;
            _trackerInfo = trackerInfo;

            // add all children to be tracked
            AddChildren(objectToTrack);

            // if this list implements the collection changed event we need to listen to it
            INotifyCollectionChanged track = objectToTrack as INotifyCollectionChanged;
            if (track != null)
            {
                track.CollectionChanged += OnCollectionChanged;
            }
        }

        /// <summary>
        ///     Handler for collection changed. It adds and removes trackers as the list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notifyCollectionChangedEventArgs"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            bool fireChanged = false;
            bool changed = false;

            if (!IsDirty)
            {
                fireChanged = true;
            }

            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    changed = AddChildren(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    changed = RemoveChildren(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    changed = RemoveChildren(notifyCollectionChangedEventArgs.OldItems);
                    changed |= AddChildren(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    changed = RemoveChildren(_componentTrackers.Values.ToArray());
                    break;
            }

            if (changed)
            {
                _isDirty = true;
            }

            if (_isDirty && fireChanged)
            {
                IsDirtyChanged?.Invoke(this, new IsDiryChangedArgs(_isDirty));
            }
        }

        /// <summary>
        ///     Remove child object that need to stop being tracked
        /// </summary>
        /// <param name="childObjects"></param>
        /// <returns></returns>
        private bool RemoveChildren(IEnumerable childObjects)
        {
            bool returnValue = false;

            // foreach child object try and find a tracker and dispose it
            foreach (object child in childObjects)
            {
                IComponentTracker oldTracker;

                if (!_componentTrackers.TryGetValue(child, out oldTracker))
                {
                    continue;
                }

                _componentTrackers.Remove(child);
                oldTracker.IsDirtyChanged -= ChildTrackerIsDirtyChanged;
                oldTracker.Dispose();

                returnValue = true;
            }

            return returnValue;
        }

        #endregion
    }
}