// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbEngine.cs" company="">
//   
// </copyright>
// <summary>
//   The db engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    /// <summary>
    /// The db engine.
    /// </summary>
    public enum DbEngine
    {
        /// <summary>
        /// The sql server.
        /// </summary>
        SqlServer, 

        /// <summary>
        /// The sql server ce.
        /// </summary>
        SqlServerCe, 

        /// <summary>
        /// The my sql.
        /// </summary>
        MySql, 

        /// <summary>
        /// The postgre sql.
        /// </summary>
        PostgreSql, 

        /// <summary>
        /// The oracle.
        /// </summary>
        Oracle, 

        /// <summary>
        /// The sq lite.
        /// </summary>
        SqLite
    }
}