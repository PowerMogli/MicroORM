using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// Leaf tracker will track every property on an object
    /// </summary>
    internal class LeafComponentTracker : IComponentTracker
    {
        private bool disposed;
        private Dictionary<string, PropertyTracker> propertyTrackers;
        private ComponentTrackerHelper helper;
        private object objectToTrack;
        private TrackerInfo trackerInfo;
        private bool isDirty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        public LeafComponentTracker(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
        {
            Initialize(helper, objectToTrack, trackerInfo);
        }

        /// <summary>
        /// The component being tracker
        /// </summary>
        public object TrackedComponent
        {
            get { return objectToTrack; }
        }

        /// <summary>
        /// True if the component or one of it's children is dirty
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                }
            }
        }

        /// <summary>
        /// mark the component and all it's children as clean
        /// </summary>
        public void MarkAsClean()
        {
            isDirty = false;

            // loop through all properties and reset the original values
            foreach (PropertyTracker oValuePair in propertyTrackers.Values)
            {
                object newValue = oValuePair.PropertyAccess(objectToTrack);

                oValuePair.SetOriginalValue(newValue);

                oValuePair.IsDirty = false;
            }
        }

        /// <summary>
        /// A list of child trackers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IComponentTracker> ChildTrackers()
        {
            return new IComponentTracker[0];
        }

        /// <summary>
        /// Event that is raised when the is dirty flag is changed
        /// </summary>
        public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

        /// <summary>
        /// Dispose of the tracker
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                if (objectToTrack is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)objectToTrack).PropertyChanged -= OnPropertyChanged;
                }

                propertyTrackers.Clear();
                disposed = true;
            }
        }

        /// <summary>
        /// Initialize the object, setting up property trackers for all relavent properties
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="objectToTrack"></param>
        /// <param name="trackerInfo"></param>
        private void Initialize(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
        {
            propertyTrackers = new Dictionary<string, PropertyTracker>();

            this.helper = helper;
            this.objectToTrack = objectToTrack;
            this.trackerInfo = trackerInfo;

            // listen to property change events
            if (objectToTrack is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)objectToTrack).PropertyChanged += OnPropertyChanged;
            }

            // loop through all public properties
            foreach (PropertyInfo propertyInfo in objectToTrack.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // only monitor properties that are write able
                if (propertyInfo.CanWrite)
                {
                    bool trackWeakly = !(propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string));

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
                    propertyTrackers[propertyInfo.Name] = propertyTracker;
                }
            }
        }

        /// <summary>
        /// This is the property changed handler for the object being tracker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            PropertyTracker propertyTrackingInfo;

            // Get the property tracker associated with the property that changed
            if (propertyTrackers.TryGetValue(propertyChangedEventArgs.PropertyName, out propertyTrackingInfo))
            {
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
                else // the new value is null and we have an original value
                {
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
                    IsDirty = propertyTrackers.Values.Any(x => x.IsDirty);
                }

                if (IsDirtyChanged != null)
                {
                    IsDirtyChanged(this, new IsDiryChangedArgs(isDirty, originalValue, newValue, propertyChangedEventArgs.PropertyName));
                }
            }
        }
    }
}