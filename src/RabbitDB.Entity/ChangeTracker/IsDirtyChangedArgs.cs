#region using directives

using System;

#endregion

// LICENCE: The Code Project Open License (CPOL) 1.02
// LICENCE TO DOWNLOAD: http://www.codeproject.com/info/CPOL.zip
// AUTHOR(S): SACHA BARBER, IAN P JOHNSON
// WHERE TO FIND ORIGINAL: http://www.codeproject.com/Articles/651464/Expression-API-Cookbook

namespace RabbitDB.Entity.ChangeTracker
{
    /// <summary>
    /// </summary>
    internal class IsDiryChangedArgs : EventArgs
    {
        #region Construction

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="isDirty">the new value for is dirty</param>
        public IsDiryChangedArgs(bool isDirty)
        {
            IsDirty = isDirty;
        }

        public IsDiryChangedArgs(bool isDirty, object oldValue, object newValue, string propertyName)
            : this(isDirty)
        {
            OldValue = oldValue;
            NewValue = newValue;
            PropertyName = propertyName;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     The new is dirty value
        /// </summary>
        public bool IsDirty { get; private set; }

        public object NewValue { get; private set; }

        public object OldValue { get; private set; }

        public string PropertyName { get; private set; }

        #endregion
    }
}