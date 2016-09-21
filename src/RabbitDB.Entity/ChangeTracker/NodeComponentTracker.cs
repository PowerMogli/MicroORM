#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;

using RabbitDB.ChangeTracker;

#endregion

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.Entity.ChangeTracker
{
    /// <summary>
    ///     NodeComponentTracker tracks a specific property on an object
    /// </summary>
    internal class NodeComponentTracker : IComponentTracker
    {
        #region Fields

        private IComponentTracker _childComponentTracker;
        private bool _disposed;
        private ComponentTrackerHelper _helper;
        private PropertyTracker _propertyTracker;

        private TrackerInfo _trackerInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Default Constructor
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        public NodeComponentTracker(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
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
            get
            {
                if (_childComponentTracker != null)
                {
                    return _propertyTracker.IsDirty || _childComponentTracker.IsDirty;
                }

                return _propertyTracker.IsDirty;
            }

            private set
            {
                if (_propertyTracker.IsDirty == value)
                {
                    return;
                }

                _propertyTracker.IsDirty = value;

                if (_childComponentTracker == null || _childComponentTracker.IsDirty == false)
                {
                    IsDirtyChanged?.Invoke(this, new IsDiryChangedArgs(_propertyTracker.IsDirty));
                }
            }
        }

        /// <summary>
        ///     The component being tracker
        /// </summary>
        public object TrackedComponent { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     A list of child trackers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComponentTracker> ChildTrackers()
        {
            if (_childComponentTracker != null)
                yield return _childComponentTracker;
        }

        /// <summary>
        ///     Dispose of the component tracker and it's children
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_childComponentTracker != null)
            {
                _childComponentTracker.Dispose();
                _childComponentTracker = null;
            }

            _disposed = true;
        }

        /// <summary>
        ///     mark the component and all it's children as clean
        /// </summary>
        public void MarkAsClean()
        {
            _propertyTracker.IsDirty = false;

            // get the current value now that it has been marked as clean
            object value = _helper.AccessProperty(TrackedComponent, _trackerInfo.PropertyName);

            // change the original value nw that we are clean
            _propertyTracker.SetOriginalValue(value);

            // mark child tracker as clean
            _childComponentTracker?.MarkAsClean();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handler for child component tracker changing is dirty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ChildComponentTrackerOnIsDirtyChanged(object sender, IsDiryChangedArgs eventArgs)
        {
            if (!_propertyTracker.IsDirty)
            {
                IsDirtyChanged?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        ///     Initialize the new tracker
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        private void Initialize(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
        {
            _helper = helper;
            TrackedComponent = objectToTrack;
            _trackerInfo = trackerInfo;

            // calculate if we should track the property weakly
            bool trackWeakly = !(trackerInfo.PropertyType.IsValueType || trackerInfo.PropertyType == typeof(string));
            object currentValue = helper.AccessProperty(objectToTrack, trackerInfo.PropertyName);

            _propertyTracker = new PropertyTracker(trackWeakly);

            _propertyTracker.SetOriginalValue(currentValue);

            // if we have a current value and child tracker info start tracking
            if (currentValue != null && trackerInfo.ChildTrackerInfo != null)
            {
                _childComponentTracker = helper.CreateTracker(currentValue, trackerInfo.ChildTrackerInfo);

                _childComponentTracker.IsDirtyChanged += ChildComponentTrackerOnIsDirtyChanged;
            }

            INotifyPropertyChanged propertyChangeObject = objectToTrack as INotifyPropertyChanged;

            if (propertyChangeObject != null)
            {
                propertyChangeObject.PropertyChanged += PropertyChangedOnTrackedObject;
            }
        }

        /// <summary>
        ///     The property change handler for the object being tracked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void PropertyChangedOnTrackedObject(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_childComponentTracker != null)
            {
                _childComponentTracker.IsDirtyChanged -= ChildComponentTrackerOnIsDirtyChanged;

                _childComponentTracker.Dispose();
                _childComponentTracker = null;
            }

            bool hasOriginalValue;
            object originalValue = _propertyTracker.GetOriginalValue(out hasOriginalValue);
            object newValue = _helper.AccessProperty(TrackedComponent, _trackerInfo.PropertyName);

            if (newValue != null)
            {
                if (originalValue != null && newValue.Equals(originalValue))
                {
                    IsDirty = false;
                }
                else
                {
                    IsDirty = true;
                }
            }
            else
            {
                IsDirty = hasOriginalValue;
            }

            // if there is a new value and we have a child tracker info we need to create a new tracker and listen to it
            if (newValue == null || _trackerInfo.ChildTrackerInfo == null)
            {
                return;
            }
            _childComponentTracker = _helper.CreateTracker(newValue, _trackerInfo.ChildTrackerInfo);

            _childComponentTracker.IsDirtyChanged += ChildComponentTrackerOnIsDirtyChanged;
        }

        #endregion
    }
}