// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfoException.cs" company="">
//   
// </copyright>
// <summary>
//   Exception is thrown if the mapping for a persistent object couldn't been constructed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception is thrown if the mapping for a persistent object couldn't been constructed.
    /// </summary>
    /// <remarks>
    /// This exception is thrown if the mapping for a persistent object couldn't been constructed. Reasons for 
    /// this excepciotn could be: the same field has been mapped to more then one property or the 
    /// <see>PersistentAttribute</see> is missing on the class.
    /// </remarks>
    [Serializable]
    public sealed class TableInfoException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfoException"/> class. 
        /// Creates a new instance of the
        /// <see cref="TableInfoException">MappingException Class</see>.
        /// </summary>
        public TableInfoException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfoException"/> class. 
        /// Creates a new instance of the
        /// <see cref="TableInfoException">MappingException Class</see>.
        /// </summary>
        /// <param name="message">
        /// A message that describes the current exception.
        /// </param>
        public TableInfoException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfoException"/> class. 
        /// Creates a new instance of the
        /// <see cref="TableInfoException">MappingException Class</see>.
        /// </summary>
        /// <param name="message">
        /// A message that describes the current exception.
        /// </param>
        /// <param name="inner">
        /// The exception that caused the current exception.
        /// </param>
        public TableInfoException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfoException"/> class. 
        /// Creates a new instance of the
        /// <see cref="TableInfoException">MappingException Class</see>.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private TableInfoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}