
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