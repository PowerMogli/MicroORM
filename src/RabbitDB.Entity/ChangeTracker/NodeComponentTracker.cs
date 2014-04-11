using System;
using System.Collections.Generic;
using System.ComponentModel;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
	/// NodeComponentTracker tracks a specific property on an object
	/// </summary>
	internal class NodeComponentTracker : IComponentTracker
	{
		private bool _disposed;
		private IComponentTracker _childComponentTracker;
		private ComponentTrackerHelper _helper;

        private TrackerInfo _trackerInfo;
		private PropertyTracker _propertyTracker;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="objectToTrack"></param>
		/// <param name="trackerInfo"></param>
		public NodeComponentTracker(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
		{
			Initialize(helper, objectToTrack, trackerInfo);
		}

        /// <summary>
        /// The component being tracker
        /// </summary>
        public object TrackedComponent { get; private set; }

        /// <summary>
		/// True if the component or one of it's children is dirty
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
			        IsDirtyChanged(this, new IsDiryChangedArgs(_propertyTracker.IsDirty));
			    }
			}
		}

		/// <summary>
		/// mark the component and all it's children as clean
		/// </summary>
		public void MarkAsClean()
		{
			_propertyTracker.IsDirty = false;

			// get the current value now that it has been marked as clean
			var value = _helper.AccessProperty(this.TrackedComponent, _trackerInfo.PropertyName);

			// change the original value nw that we are clean
			_propertyTracker.SetOriginalValue(value);

			// mark child tracker as clean
			if (_childComponentTracker != null)
			{
				_childComponentTracker.MarkAsClean();
			}
		}

		/// <summary>
		/// A list of child trackers
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IComponentTracker> ChildTrackers()
		{
			if (_childComponentTracker != null)
				yield return _childComponentTracker;
		}

		/// <summary>
		/// Event that is raised when the is dirty flag is changed
		/// </summary>
		public event EventHandler<IsDiryChangedArgs> IsDirtyChanged;

		/// <summary>
		/// Dispose of the component tracker and it's children
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
		/// Initialize the new tracker
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
			var trackWeakly = !(trackerInfo.PropertyType.IsValueType || trackerInfo.PropertyType == typeof(string));
			var currentValue = helper.AccessProperty(objectToTrack, trackerInfo.PropertyName);

			_propertyTracker = new PropertyTracker(trackWeakly);

			_propertyTracker.SetOriginalValue(currentValue);

			// if we have a current value and child tracker info start tracking
			if (currentValue != null && trackerInfo.ChildTrackerInfo != null)
			{
				_childComponentTracker = helper.CreateTracker(currentValue, trackerInfo.ChildTrackerInfo);

				_childComponentTracker.IsDirtyChanged += ChildComponentTrackerOnIsDirtyChanged;
			}

			var propertyChangeObject = objectToTrack as INotifyPropertyChanged;

			if (propertyChangeObject != null)
			{
				propertyChangeObject.PropertyChanged += PropertyChangedOnTrackedObject;
			}
		}

		/// <summary>
		/// Handler for child component tracker changing is dirty
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ChildComponentTrackerOnIsDirtyChanged(object sender, IsDiryChangedArgs eventArgs)
		{
		    if (!_propertyTracker.IsDirty && IsDirtyChanged != null)
			{
				IsDirtyChanged(this, eventArgs);
			}
		}

		/// <summary>
		/// The property change handler for the object being tracked
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
			var originalValue = _propertyTracker.GetOriginalValue(out hasOriginalValue);
			var newValue = _helper.AccessProperty(TrackedComponent, _trackerInfo.PropertyName);

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
				this.IsDirty = hasOriginalValue;
			}

			// if there is a new value and we have a child tracker info we need to create a new tracker and listen to it
		    if (newValue == null || _trackerInfo.ChildTrackerInfo == null)
		    {
		        return;
		    }
		    _childComponentTracker = _helper.CreateTracker(newValue, _trackerInfo.ChildTrackerInfo);

		    _childComponentTracker.IsDirtyChanged += this.ChildComponentTrackerOnIsDirtyChanged;
		}
	}
}