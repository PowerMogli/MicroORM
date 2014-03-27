using System;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// PropertyTracker is used by LeafComponentTracker to store information about a property that is being tracked
    /// </summary>
    internal class PropertyTracker
    {
        private bool hasOriginalValue;
        private bool trackOriginalValueWeakly;
        private object propertyValue;

        public PropertyTracker(bool trackOriginalValueWeakly)
        {
            this.trackOriginalValueWeakly = trackOriginalValueWeakly;
        }

        public Func<object, object> PropertyAccess { get; set; }

        public bool IsDirty { get; set; }

        /// <summary>
        /// Get the recorded original value
        /// </summary>
        /// <param name="hasOriginalValueOut"></param>
        /// <returns></returns>
        public object GetOriginalValue(out bool hasOriginalValueOut)
        {
            hasOriginalValueOut = hasOriginalValue;

            if (trackOriginalValueWeakly)
            {
                if (hasOriginalValue)
                {
                    return ((WeakReference)propertyValue).Target;
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Sets an original value for the property
        /// </summary>
        /// <param name="newValue"></param>
        public void SetOriginalValue(object newValue)
        {
            if (newValue == null)
            {
                hasOriginalValue = false;
                propertyValue = null;
            }
            else
            {
                if (trackOriginalValueWeakly)
                {
                    propertyValue = new WeakReference(newValue);
                }
                else
                {
                    propertyValue = newValue;
                }
            }
        }
    }
}