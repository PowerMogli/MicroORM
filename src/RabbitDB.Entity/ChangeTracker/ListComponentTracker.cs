using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace RabbitDB.ChangeTracker
{
	/// <summary>
	/// ListComponentTracker is used to track IEnumerable objects
	/// </summary>
	internal class ListComponentTracker : IComponentTracker
	{
		private bool disposed;
		private Dictionary<object, IComponentTracker> componentTrackers;
		private ComponentTrackerHelper helper;
		private IEnumerable objectToTrack;
		private TrackerInfo trackerInfo;
		private bool isDirty;

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
			get { return objectToTrack; }
		}

		/// <summary>
		/// True if the component or one of it's children is dirty
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return isDirty || componentTrackers.Values.Any(x => x.IsDirty);
			}
		}

		/// <summary>
		/// mark the component and all it's children as clean
		/// </summary>
		public void MarkAsClean()
		{
			isDirty = false;

			foreach (IComponentTracker componentTracker in componentTrackers.Values)
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
			return componentTrackers.Values;
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
			if (!disposed)
			{
				foreach (IComponentTracker componentTracker in componentTrackers.Values)
				{
					componentTracker.Dispose();
				}
				componentTrackers = null;
				disposed = true;
			}
		}

		/// <summary>
		/// Initialize the tracker
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="objectToTrack"></param>
		/// <param name="trackerInfo"></param>
		private void Initialize(ComponentTrackerHelper helper, IEnumerable objectToTrack, TrackerInfo trackerInfo)
		{
			componentTrackers = new Dictionary<object, IComponentTracker>();

			this.helper = helper;
			this.objectToTrack = objectToTrack;
			this.trackerInfo = trackerInfo;

			// add all children to be tracked
			AddChildren(objectToTrack);

			// if this list implements the collection changed event we need to listen to it
			if (objectToTrack is INotifyCollectionChanged)
			{
				((INotifyCollectionChanged)objectToTrack).CollectionChanged += OnCollectionChanged;
			}
		}

		/// <summary>
		/// Handler for collection changed. It adds and removes trackers as the list changes
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
					changed = RemoveChildren(componentTrackers.Values.ToArray());
					break;
			}

			if (changed)
			{
				isDirty = true;
			}

			if (isDirty && fireChanged && IsDirtyChanged != null)
			{
				IsDirtyChanged(this, new IsDiryChangedArgs(isDirty));
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
			if (isDirty)
			{
				return;
			}

			IComponentTracker childTracker = (IComponentTracker)sender;
			bool newValue = childTracker.IsDirty;
			bool fireChange = true;

			foreach (KeyValuePair<object, IComponentTracker> kvp in componentTrackers)
			{
				if (kvp.Key == sender)
				{
					continue;
				}

				if (kvp.Value.IsDirty != newValue)
				{
					fireChange = false;
					break;
				}
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
			bool returnValue = false;
			IListComponentFilter listFilter = trackerInfo.ListFilter;

			// if we have child tracker info create a new tracker for the object
			if (trackerInfo.ChildTrackerInfo != null)
			{
				foreach (object child in childObjects)
				{
					// filter the object if a filter exists
					if (listFilter != null && !listFilter.FilterComponent(child))
					{
						continue;
					}

					IComponentTracker childTracker =
						helper.CreateTracker(child, trackerInfo.ChildTrackerInfo);

					childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

					componentTrackers.Add(child, childTracker);

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
						helper.CreateTracker(child, new TrackerInfo { TrackerType = ComponentTrackerType.Leaf });

					childTracker.IsDirtyChanged += ChildTrackerIsDirtyChanged;

					componentTrackers.Add(child, childTracker);

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
			bool returnValue = false;

			// foreach child object try and find a tracker and dispose it
			foreach (object child in childObjects)
			{
				IComponentTracker oldTracker;

				if (componentTrackers.TryGetValue(child, out oldTracker))
				{
					componentTrackers.Remove(child);
					oldTracker.IsDirtyChanged -= ChildTrackerIsDirtyChanged;
					oldTracker.Dispose();

					returnValue = true;
				}
			}

			return returnValue;
		}
	}
}