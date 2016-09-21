// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcedureParameterCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The procedure parameter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using RabbitDB.Contracts.Query.StoredProcedure;

#endregion

namespace RabbitDB.Query.StoredProcedure
{
    /// <summary>
    ///     The procedure parameter collection.
    /// </summary>
    internal class ProcedureParameterCollection : IProcedureParameterCollection
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcedureParameterCollection" /> class.
        /// </summary>
        internal ProcedureParameterCollection()
        {
            Parameters = new Dictionary<string, IDbDataParameter>();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        internal Dictionary<string, IDbDataParameter> Parameters { get; }

        /// <summary>
        ///     The
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbDataParameter" />.
        /// </returns>
        public IDbDataParameter this[string key] => Parameters[key];

        #endregion

        #region Public Methods

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return Parameters.Values.GetEnumerator();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public void Add(string key, IDbDataParameter parameter)
        {
            Parameters.Add(key, parameter);
        }

        /// <summary>
        ///     The contains key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return Parameters.ContainsKey(key);
        }

        /// <summary>
        ///     The get parameter value.
        /// </summary>
        /// <param name="parameterName">
        ///     The parameter name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public T GetParameterValue<T>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            IDbDataParameter parameter = Parameters[parameterName];

            if (parameter == null)
            {
                return default(T);
            }

            return (T)parameter.Value;
        }

        /// <summary>
        ///     The is parameter valid.
        /// </summary>
        /// <param name="parameterName">
        ///     The parameter name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="dbType">
        ///     The db type.
        /// </param>
        /// <param name="length">
        ///     The length.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public bool IsParameterValid<T>(string parameterName, T value, DbType dbType, int length)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            string stringValue = value as string;

            return string.IsNullOrWhiteSpace(stringValue) || length <= 0 || stringValue.Length <= length;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator<IDbDataParameter> IEnumerable<IDbDataParameter>.GetEnumerator()
        {
            return (IEnumerator<IDbDataParameter>)GetEnumerator();
        }

        #endregion
    }
}