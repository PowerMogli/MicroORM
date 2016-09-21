// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INullValueResolver.cs" company="">
//   
// </copyright>
// <summary>
//   The NullValueResolver interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace RabbitDB.Contracts.Storage
{
    /// <summary>
    /// The NullValueResolver interface.
    /// </summary>
    internal interface INullValueResolver
    {
        #region Public Methods and Operators

        /// <summary>
        /// The resolve null value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object ResolveNullValue(object value, Type type);

        #endregion
    }
}