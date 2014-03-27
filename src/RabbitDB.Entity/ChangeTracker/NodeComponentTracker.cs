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
		private bool disposed;
		private IComponentTracker childComponentTracker;
		private ComponentTrackerHelper helper;
		private object objectToTrack;
		private TrackerInfo trackerInfo;
		private PropertyTracker propertyTracker;

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
				if (childComponentTracker != null)
				{
					return propertyTracker.IsDirty || childComponentTracker.IsDirty;
				}

				return propertyTracker.IsDirty;
			}
			private set
			{
				if (propertyTracker.IsDirty != value)
				{
					propertyTracker.IsDirty = value;

					if (childComponentTracker == null || childComponentTracker.IsDirty == false)
					{
						IsDirtyChanged(this, new IsDiryChangedArgs(propertyTracker.IsDirty));
					}
				}
			}
		}

		/// <summary>
		/// mark the component and all it's children as clean
		/// </summary>
		public void MarkAsClean()
		{
			propertyTracker.IsDirty = false;

			// get the current value now that it has been marked as clean
			object value = helper.AccessProperty(objectToTrack, trackerInfo.PropertyName);

			// change the original value nw that we are clean
			propertyTracker.SetOriginalValue(value);

			// mark child tracker as clean
			if (childComponentTracker != null)
			{
				childComponentTracker.MarkAsClean();
			}
		}

		/// <summary>
		/// A list of child trackers
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IComponentTracker> ChildTrackers()
		{
			if (childComponentTracker != null)
				yield return childComponentTracker;
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
			if (!disposed)
			{
				if (childComponentTracker != null)
				{
					childComponentTracker.Dispose();
					childComponentTracker = null;
				}

				disposed = true;
			}
		}

		/// <summary>
		/// Initialize the new tracker
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="objectToTrack"></param>
		/// <param name="trackerInfo"></param>
		private void Initialize(ComponentTrackerHelper helper, object objectToTrack, TrackerInfo trackerInfo)
		{
			this.helper = helper;
			this.objectToTrack = objectToTrack;
			this.trackerInfo = trackerInfo;

			// calculate if we should track the property weakly
			bool trackWeakly = !(trackerInfo.PropertyType.IsValueType || trackerInfo.PropertyType == typeof(string));
			object currentValue = helper.AccessProperty(objectToTrack, trackerInfo.PropertyName);

			propertyTracker = new PropertyTracker(trackWeakly);

			propertyTracker.SetOriginalValue(currentValue);

			// if we have a current value and child tracker info start tracking
			if (currentValue != null && trackerInfo.ChildTrackerInfo != null)
			{
				childComponentTracker = helper.CreateTracker(currentValue, trackerInfo.ChildTrackerInfo);

				childComponentTracker.IsDirtyChanged += ChildComponentTrackerOnIsDirtyChanged;
			}

			INotifyPropertyChanged propertyChangeObject = objectToTrack as INotifyPropertyChanged;

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
			IComponentTracker componentTracker = sender as IComponentTracker;

			if (!propertyTracker.IsDirty && IsDirtyChanged != null)
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
			if (childComponentTracker != null)
			{
				childComponentTracker.IsDirtyChanged -= ChildComponentTrackerOnIsDirtyChanged;

				childComponentTracker.Dispose();
				childComponentTracker = null;
			}

			bool hasOriginalValue;
			object originalValue = propertyTracker.GetOriginalValue(out hasOriginalValue);
			object newValue = helper.AccessProperty(objectToTrack, trackerInfo.PropertyName);

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
				if (hasOriginalValue)
				{
					IsDirty = true;
				}
				else
				{
					IsDirty = false;
				}
			}

			// if there is a new value and we have a child tracker info we need to create a new tracker and listen to it
			if (newValue != null && trackerInfo.ChildTrackerInfo != null)
			{
				childComponentTracker = helper.CreateTracker(newValue, trackerInfo.ChildTrackerInfo);

				childComponentTracker.IsDirtyChanged += ChildComponentTrackerOnIsDirtyChanged;
			}
		}
	}
}