// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbForeignKey.cs" company="">
//   
// </copyright>
// <summary>
//   The db foreign key.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    /// <summary>
    /// The db foreign key.
    /// </summary>
    internal class DbForeignKey
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the from column.
        /// </summary>
        public string FromColumn { get; set; }

        /// <summary>
        /// Gets or sets the to column.
        /// </summary>
        public string ToColumn { get; set; }

        /// <summary>
        /// Gets or sets the to table.
        /// </summary>
        public string ToTable { get; set; }

        #endregion
    }
}