
// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
	/// <summary>
	/// Defines different types of component trackers
	/// </summary>
	internal enum ComponentTrackerType
	{
		/// <summary>
		/// A node tracker monitors one property on an object
		/// </summary>
		Node,

		/// <summary>
		/// List tracker monitors a list for changes
		/// </summary>
		List,

		/// <summary>
		/// Leaf tracker will monitor all properties on an object
		/// </summary>
		Leaf
	}
}