using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// ListComponentTracker is used to track IEnumerable objects
    /// </summary>
    internal class ListComponentTracker : IComponentTracker
    {
        private bool _disposed;
        private Dictionary<object, IComponentTracker> _componentTrackers;
        private ComponentTrackerHelper _helper;
        private IEnumerable _objectToTrack;
        private TrackerInfo _trackerInfo;
        private bool _isDirty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        public ListComponentTracker(ComponentTrackerHelper helper, IEnumerable objectToTrack, TrackerInfo trackerInfo)
        {
            Initialize(helper, objectToTrack, trackerInfo);
        }

        /// <summary>
        /// The component being tracker
        /// </summary>
        public object TrackedComponent
        {
            get { return _objectToTrack; }
        }

        /// <summary>
        /// True if the component or one of it's children is dirty
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty || _componentTrackers.Values.Any(x => x.IsDirty);
            }
        }

        /// <summary>
        /// mark the component and all it's children as clean
        /// </summary>
        public void MarkAsClean()
        {
            _isDirty = false;

            foreach (var componentTracker in _componentTrackers.Values)
            {
                componentTracker.MarkAsClean();
            }
        }

        /// <summary>
        /// A list of child trackers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComponentTracker> ChildTrackers()
        {
            return _componentTrackers.Values;
        }

        /// <summary>
        /// Event that is raised when the is dirty flag is changed
        /// </summary>
        public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

        /// <summary>
        /// Dispose the tracker
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var componentTracker in _componentTrackers.Values)
            {
                componentTracker.Dispose();
            }
            _componentTrackers = null;
            _disposed = true;
        }

        /// <summary>
        /// Initialize the tracker
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
            var track = objectToTrack as INotifyCollectionChanged;
            if (track != null)
            {
                track.CollectionChanged += OnCollectionChanged;
            }
        }

        /// <summary>
        /// Handler for collection changed. It adds and removes trackers as the list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notifyCollectionChangedEventArgs"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var fireChanged = false;
            var changed = false;

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

            if (_isDirty && fireChanged && IsDirtyChanged != null)
            {
                IsDirtyChanged(this, new IsDiryChangedArgs(_isDirty));
            }
        }

        /// <summary>
        /// Handler for child tracker is dirty changing
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

            var childTracker = (IComponentTracker)sender;
            var newValue = childTracker.IsDirty;
            var fireChange = true;

            foreach (var kvp in this._componentTrackers)
            {
                if (kvp.Key == sender
                    || kvp.Value.IsDirty == newValue)
                {
                    continue;
                }

                fireChange = false;
                break;
            }

            if (fireChange && IsDirtyChanged != null)
            {
                IsDirtyChanged(this, eventArgs);
            }
        }

        /// <summary>
        /// Add children to be tracked
        /// </summary>
        /// <param name="childObjects"></param>
        /// <returns></returns>
        private bool AddChildren(IEnumerable childObjects)
        {
            var returnValue = false;
            var listFilter = _trackerInfo.ListFilter;

            // if we have child tracker info create a new tracker for the object
            if (_trackerInfo.ChildTrackerInfo != null)
            {
                foreach (var child in childObjects)
                {
                    // filter the object if a filter exists
                    if (listFilter != null && !listFilter.FilterComponent(child))
                    {
                        continue;
                    }

                    var childTracker =
                        _helper.CreateTracker(child, _trackerInfo.ChildTrackerInfo);

                    childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

                    _componentTrackers.Add(child, childTracker);

                    returnValue = true;
                }
            }
            else
            {
                // Create a leaf tracker for each child in the list
                foreach (var child in childObjects)
                {
                    if (listFilter != null && !listFilter.FilterComponent(child))
                    {
                        continue;
                    }

                    var childTracker =
                        _helper.CreateTracker(child, new TrackerInfo { TrackerType = ComponentTrackerType.Leaf });

                    childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

                    _componentTrackers.Add(child, childTracker);

                    returnValue = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Remove child object that need to stop being tracked
        /// </summary>
        /// <param name="childObjects"></param>
        /// <returns></returns>
        private bool RemoveChildren(IEnumerable childObjects)
        {
            var returnValue = false;

            // foreach child object try and find a tracker and dispose it
            foreach (var child in childObjects)
            {
                IComponentTracker oldTracker;

                if (!_componentTrackers.TryGetValue(child, out oldTracker))
                {
                    continue;
                }
                
                _componentTrackers.Remove(child);
                oldTracker.IsDirtyChanged -= this.ChildTrackerIsDirtyChanged;
                oldTracker.Dispose();

                returnValue = true;
            }

            return returnValue;
        }
    }
}