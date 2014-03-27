using System;
using System.Collections.Generic;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
	/// <summary>
	/// IComponentTracker is the interface all component trackers implements.
	/// </summary>
	internal interface IComponentTracker : IDisposable
	{
		/// <summary>
		/// The component being tracker
		/// </summary>
		object TrackedComponent { get; }

		/// <summary>
		/// True if the component or one of it's children is dirty
		/// </summary>
		bool IsDirty { get; }

		/// <summary>
		/// mark the component and all it's children as clean
		/// </summary>
		void MarkAsClean();

		/// <summary>
		/// A list of child trackers
		/// </summary>
		/// <returns></returns>
		IEnumerable<IComponentTracker> ChildTrackers();

		/// <summary>
		/// Event that is raised when the is dirty flag is changed
		/// </summary>
		event EventHandler<IsDiryChangedArgs> IsDirtyChanged;
	}
}