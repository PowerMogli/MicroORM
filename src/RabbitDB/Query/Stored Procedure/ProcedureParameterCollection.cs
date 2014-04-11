// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcedureParameterCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The procedure parameter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.StoredProcedure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// The procedure parameter collection.
    /// </summary>
    internal class ProcedureParameterCollection : IEnumerable<IDbDataParameter>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureParameterCollection"/> class.
        /// </summary>
        internal ProcedureParameterCollection()
        {
            this.Parameters = new Dictionary<string, IDbDataParameter>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        internal Dictionary<string, IDbDataParameter> Parameters { get; private set; }

        #endregion

        #region Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDbDataParameter"/>.
        /// </returns>
        internal IDbDataParameter this[string key]
        {
            get
            {
                return this.Parameters[key];
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return this.Parameters.Values.GetEnumerator();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator<IDbDataParameter> IEnumerable<IDbDataParameter>.GetEnumerator()
        {
            return (IEnumerator<IDbDataParameter>)this.GetEnumerator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        internal void Add(string key, IDbDataParameter parameter)
        {
            this.Parameters.Add(key, parameter);
        }

        /// <summary>
        /// The contains key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool ContainsKey(string key)
        {
            return this.Parameters.ContainsKey(key);
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
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal T GetParameterValue<T>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var parameter = this.Parameters[parameterName];
            if (parameter == null)
            {
                return default(T);
            }

            return (T)parameter.Value;
        }

        /// <summary>
        /// The is parameter valid.
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
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal bool IsParameterValid<T>(string parameterName, T value, DbType dbType, int length)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new ArgumentNullException("value");
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var stringValue = value as string;

            return string.IsNullOrWhiteSpace(stringValue) || length <= 0 || stringValue.Length <= length;
        }

        #endregion
    }
}