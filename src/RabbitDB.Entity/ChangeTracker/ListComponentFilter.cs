using System;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
	/// <summary>
	/// ListComponentFilter is used by the ListComponentTracker to filter the undesirables out 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class ListComponentFilter<T> : IListComponentFilter
	{
		private readonly Func<T, bool> filterMethod;
 
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="filterMethod"></param>
		public ListComponentFilter(Func<T, bool> filterMethod)
		{
			this.filterMethod = filterMethod;
		}

		/// <summary>
		/// called for each item in the list
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public bool FilterComponent(object component)
		{
			return filterMethod((T)component);
		}
	}
}