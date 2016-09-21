// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeafComponentTracker.cs" company="">
//   
// </copyright>
// <summary>
//   Leaf tracker will track every property on an object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/***********************************************************************************************************************
 * LICENCE: The Code Project Open License (CPOL) 1.02
 * LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
 * AUTHOR(S): SACHA BARBER, IAN P JOHNSON
 * WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook 
 ***********************************************************************************************************************/

#region using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using RabbitDB.ChangeTracker;

#endregion

namespace RabbitDB.Entity.ChangeTracker
{
    /// <summary>
    ///     Leaf tracker will track every property on an object
    /// </summary>
    internal class LeafComponentTracker : IComponentTracker
    {
        #region Fields

        /// <summary>
        ///     The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     The _is dirty.
        /// </summary>
        private bool _isDirty;

        /// <summary>
        ///     The _property trackers.
        /// </summary>
        private Dictionary<string, PropertyTracker> _propertyTrackers;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="LeafComponentTracker" /> class.
        ///     Default constructor
        /// </summary>
        /// <param name="helper">
        /// </param>
        /// <param name="objectToTrack">
        /// </param>
        /// <param name="trackerInfo">
        /// </param>
        public LeafComponentTracker(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
        {
            Initialize(helper, objectToTrack);
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
            get { return _isDirty; }

            set
            {
                // ReSharper disable once RedundantCheckBeforeAssignment
                if (_isDirty != value)
                {
                    _isDirty = value;
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
        /// <returns>
        ///     The <see cref="IEnumerable{IComponentTracker}" />.
        /// </returns>
        public IEnumerable<IComponentTracker> ChildTrackers()
        {
            return new IComponentTracker[0];
        }

        /// <summary>
        ///     Dispose of the tracker
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            INotifyPropertyChanged changed = TrackedComponent as INotifyPropertyChanged;
            if (changed != null)
            {
                changed.PropertyChanged -= OnPropertyChanged;
            }

            _propertyTrackers.Clear();
            _disposed = true;
        }

        /// <summary>
        ///     mark the component and all it's children as clean
        /// </summary>
        public void MarkAsClean()
        {
            _isDirty = false;

            // loop through all properties and reset the original values
            foreach (PropertyTracker oValuePair in _propertyTrackers.Values)
            {
                object newValue = oValuePair.PropertyAccess(TrackedComponent);

                oValuePair.SetOriginalValue(newValue);

                oValuePair.IsDirty = false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Initialize the object, setting up property trackers for all relavent properties
        /// </summary>
        /// <param name="helper">
        /// </param>
        /// <param name="objectToTrack">
        /// </param>
        private void Initialize(ComponentTrackerHelper helper, object objectToTrack)
        {
            _propertyTrackers = new Dictionary<string, PropertyTracker>();

            TrackedComponent = objectToTrack;

            // listen to property change events
            INotifyPropertyChanged track = objectToTrack as INotifyPropertyChanged;
            if (track != null)
            {
                track.PropertyChanged += OnPropertyChanged;
            }

            // loop through all public properties
            foreach (PropertyInfo propertyInfo in objectToTrack.GetType()
                                                               .GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // only monitor properties that are write able
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }

                bool trackWeakly =
                    !(propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string));

                // create a new property tracking object and a property accessor delegate
                PropertyTracker propertyTracker = new PropertyTracker(trackWeakly)
                {
                    PropertyAccess =
                        helper.GetPropertyAccessor(objectToTrack, propertyInfo.Name)
                };

                // use the new property access delegate to retrieve the property from the object
                object origValue = propertyTracker.PropertyAccess(objectToTrack);

                // store the original value 
                propertyTracker.SetOriginalValue(origValue);

                // save the new property tracker into the dictionary by property name
                _propertyTrackers[propertyInfo.Name] = propertyTracker;
            }
        }

        /// <summary>
        ///     This is the property changed handler for the object being tracker
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="propertyChangedEventArgs">
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            PropertyTracker propertyTrackingInfo;

            // Get the property tracker associated with the property that changed
            if (!_propertyTrackers.TryGetValue(propertyChangedEventArgs.PropertyName, out propertyTrackingInfo))
            {
                return;
            }

            bool dirtyChanged = false;
            bool hasOriginalValue;
            object originalValue = propertyTrackingInfo.GetOriginalValue(out hasOriginalValue);
            object newValue = propertyTrackingInfo.PropertyAccess(sender);

            if (newValue != null)
            {
                // Property is reverting back to original value so setting is dirty to false
                if (newValue.Equals(originalValue))
                {
                    propertyTrackingInfo.IsDirty = false;

                    dirtyChanged = true;
                }

                // else if the property is clean we need to dirty it up
                else if (!propertyTrackingInfo.IsDirty)
                {
                    propertyTrackingInfo.IsDirty = true;

                    dirtyChanged = true;
                }
            }
            else if (!hasOriginalValue)
            {
                // the original value was null and the new value is null so the property is now clean
                if (propertyTrackingInfo.IsDirty)
                {
                    propertyTrackingInfo.IsDirty = false;

                    dirtyChanged = true;
                }
            }
            else
            {
                // the new value is null and we have an original value
                // only set to true if the property is clean
                if (!propertyTrackingInfo.IsDirty)
                {
                    propertyTrackingInfo.IsDirty = true;

                    dirtyChanged = true;
                }
            }

            // only check for dirty properties if the property dirty flag changed
            if (dirtyChanged)
            {
                IsDirty = _propertyTrackers.Values.Any(x => x.IsDirty);
            }

            IsDirtyChanged?.Invoke(this, new IsDiryChangedArgs(_isDirty, originalValue, newValue, propertyChangedEventArgs.PropertyName));
        }

        #endregion
    }
}