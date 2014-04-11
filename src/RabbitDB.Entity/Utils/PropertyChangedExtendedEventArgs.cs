// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedExtendedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The property changed extended event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RabbitDB.Utils
{
    using System.ComponentModel;

    /// <summary>
    /// The property changed extended event args.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedExtendedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public virtual T NewValue { get; private set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public virtual T OldValue { get; private set; }

        #endregion
    }
}