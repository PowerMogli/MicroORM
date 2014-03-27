
// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// Used by ListComponentTracker to filter the undesirables out
    /// </summary>
    internal interface IListComponentFilter
    {
        /// <summary>
        /// called for each item in the list
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool FilterComponent(object component);
    }
}