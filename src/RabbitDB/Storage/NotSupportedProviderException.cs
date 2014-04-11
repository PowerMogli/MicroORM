// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedProviderException.cs" company="">
//   
// </copyright>
// <summary>
//   The not supported provider exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System;

    /// <summary>
    /// The not supported provider exception.
    /// </summary>
    public class NotSupportedProviderException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedProviderException"/> class.
        /// </summary>
        internal NotSupportedProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedProviderException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal NotSupportedProviderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotSupportedProviderException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        internal NotSupportedProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}