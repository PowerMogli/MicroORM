
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