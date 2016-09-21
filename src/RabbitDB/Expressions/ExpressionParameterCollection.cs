// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionParameterCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The expression parameter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Expressions
{
    using System.Collections.Generic;

    /// <summary>
    /// The expression parameter collection.
    /// </summary>
    internal class ExpressionParameterCollection
    {
        #region Fields

        /// <summary>
        /// The _params.
        /// </summary>
        private readonly List<object> _params = new List<object>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the next index.
        /// </summary>
        internal int NextIndex
        {
            get
            {
                return _params.Count;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        internal void Add(object value)
        {
            _params.Add(value);
        }

        /// <summary>
        /// The to array.
        /// </summary>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        internal object[] ToArray()
        {
            return _params.ToArray();
        }

        #endregion
    }
}