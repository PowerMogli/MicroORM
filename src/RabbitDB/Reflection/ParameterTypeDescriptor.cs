// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeDescriptor.cs" company="">
//   
// </copyright>
// <summary>
//   The parameter type descriptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Reflection
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// The parameter type descriptor.
    /// </summary>
    internal class ParameterTypeDescriptor
    {
        #region Methods

        /// <summary>
        /// The to key value pairs.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>KeyValuePair</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static KeyValuePair<string, object>[] ToKeyValuePairs(object[] arguments)
        {
            var result = new Dictionary<string, object>();
            if (arguments == null)
            {
                return result.ToArray();
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(arguments[0]))
            {
                result.Add(property.Name, property.GetValue(arguments[0]));
            }

            return result.ToArray();
        }

        #endregion
    }
}