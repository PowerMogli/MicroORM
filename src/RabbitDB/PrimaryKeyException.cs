// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrimaryKeyException.cs" company="">
//   
// </copyright>
// <summary>
//   The primary key exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB
{
    using System;

    /// <summary>
    /// The primary key exception.
    /// </summary>
    public class PrimaryKeyException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyException"/> class.
        /// </summary>
        internal PrimaryKeyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal PrimaryKeyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        internal PrimaryKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}