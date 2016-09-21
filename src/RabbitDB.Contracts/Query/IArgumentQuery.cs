// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArgumentQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The ArgumentQuery interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Contracts.Query
{
    /// <summary>
    /// The ArgumentQuery interface.
    /// </summary>
    internal interface IArgumentQuery
    {
        #region Public Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        IQueryParameterCollection Arguments { get; }

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        string SqlStatement { get; }

        #endregion
    }
}