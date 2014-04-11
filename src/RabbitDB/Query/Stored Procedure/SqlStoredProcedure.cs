// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStoredProcedure.cs" company="">
//   
// </copyright>
// <summary>
//   The sql stored procedure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.StoredProcedure
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// The sql stored procedure.
    /// </summary>
    public class SqlStoredProcedure : StoredProcedure
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedure"/> class.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        public SqlStoredProcedure(string storedProcedureName)
            : base(storedProcedureName)
        {
        }

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
        protected override bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1)
        {
            return AddParameter<T>(parameterName, value, dbType, length, ParameterDirection.Input);
        }

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
        /// <param name="parameterDirection">
        /// The parameter direction.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool AddParameter<T>(
            string parameterName, 
            T value, 
            DbType dbType, 
            int length = -1, 
            ParameterDirection parameterDirection = default(ParameterDirection))
        {
            if (!base.Parameters.IsParameterValid(parameterName, value, dbType, length))
            {
                return false;
            }

            var prefix = "@";
            if (parameterName.StartsWith("@"))
            {
                prefix = string.Empty;
            }

            var parameter = new SqlParameter(prefix + parameterName, value) { DbType = dbType };
            if (length > 0)
            {
                parameter.Size = length;
            }

            if (base.Parameters.ContainsKey(prefix + parameterName.ToLower()))
            {
                base.Parameters[prefix + parameterName.ToLower()].Value = value;
            }
            else
            {
                base.Parameters.Add(prefix + parameterName.ToLower(), parameter);
            }

            return true;
        }

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
        protected override T GetParameterValue<T>(string parameterName)
        {
            return this.Parameters.GetParameterValue<T>(parameterName);
        }

        #endregion
    }
}