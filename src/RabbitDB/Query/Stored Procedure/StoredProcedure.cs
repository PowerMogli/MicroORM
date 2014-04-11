// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedure.cs" company="">
//   
// </copyright>
// <summary>
//   The stored procedure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.StoredProcedure
{
    using System.Data;

    /// <summary>
    /// The stored procedure.
    /// </summary>
    public abstract class StoredProcedure
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        internal StoredProcedure(string storedProcedureName)
        {
            this.StoredProcedureName = storedProcedureName;
            this.Parameters = new ProcedureParameterCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        internal ProcedureParameterCollection Parameters { get; private set; }

        /// <summary>
        /// Gets the stored procedure name.
        /// </summary>
        internal string StoredProcedureName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The add parameter.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected abstract bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1);

        /// <summary>
        /// The get parameter value.
        /// </summary>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected abstract T GetParameterValue<T>(string parameterName);

        #endregion
    }
}