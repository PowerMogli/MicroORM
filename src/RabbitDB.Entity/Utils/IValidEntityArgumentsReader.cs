// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidEntityArgumentsReader.cs" company="">
//   
// </copyright>
// <summary>
//   The ValidEntityArgumentsReader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RabbitDB.Utils
{
    using System.Collections.Generic;

    /// <summary>
    /// The ValidEntityArgumentsReader interface.
    /// </summary>
    internal interface IValidEntityArgumentsReader
    {
        #region Public Methods and Operators

        /// <summary>
        /// The read valid entity arguments.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEnumerable<KeyValuePair<string, object>> ReadValidEntityArguments();

        #endregion
    }
}