using System;

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