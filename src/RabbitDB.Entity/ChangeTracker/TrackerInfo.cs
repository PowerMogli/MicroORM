using System;

namespace RabbitDB.ChangeTracker
{
	internal class TrackerInfo
	{
		public string PropertyName { get; set; }

		public Type PropertyType { get; set; }

		public IListComponentFilter ListFilter { get; set; }

		public ComponentTrackerType TrackerType { get; set; }

		public TrackerInfo ChildTrackerInfo { get; set; }
	}
}