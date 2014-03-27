using System;

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.ChangeTracker
{
    /// <summary>
    /// 
    /// </summary>
    internal class IsDiryChangedArgs : EventArgs
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="isDirty">the new value for is dirty</param>
        public IsDiryChangedArgs(bool isDirty)
        {
            IsDirty = isDirty;
        }

        public IsDiryChangedArgs(bool isDirty, object oldValue, object newValue, string propertyName)
            : this(isDirty)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// The new is dirty value
        /// </summary>
        public bool IsDirty { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public string PropertyName { get; private set; }
    }
}