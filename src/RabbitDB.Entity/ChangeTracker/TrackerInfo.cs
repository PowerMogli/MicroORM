using System;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

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