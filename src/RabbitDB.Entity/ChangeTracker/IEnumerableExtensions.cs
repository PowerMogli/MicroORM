using System;
using System.Collections.Generic;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// These extension exist to bridge the gap when monitoring a property on an object inside a list
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// This method is used to bridge the gap between a list item and monitoring its property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T TrackList<T>(this IEnumerable<T> list)
        {
            throw new NotImplementedException("This method should not be called only here for Fluent purposes");
        }

        /// <summary>
        /// This method can be used to filter which objects in the list should be monitored
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="filterMethod"></param>
        /// <returns></returns>
        public static T TrackList<T>(this IEnumerable<T> list, Func<T, bool> filterMethod)
        {
            throw new NotImplementedException("This method should not be called only here for Fluent purposes");
        }
    }
}