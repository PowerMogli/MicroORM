// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidTypeException.cs" company="">
//   
// </copyright>
// <summary>
//   The invalid type exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB
{
    using System;

    /// <summary>
    /// The invalid type exception.
    /// </summary>
    internal class InvalidTypeException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeException"/> class.
        /// </summary>
        internal InvalidTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal InvalidTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        internal InvalidTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}