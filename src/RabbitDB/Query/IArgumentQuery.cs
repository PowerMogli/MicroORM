// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArgumentQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The ArgumentQuery interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    /// <summary>
    /// The ArgumentQuery interface.
    /// </summary>
    interface IArgumentQuery
    {
        #region Public Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        QueryParameterCollection Arguments { get; }

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        string SqlStatement { get; }

        #endregion
    }
}