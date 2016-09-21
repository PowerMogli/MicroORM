// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbCommandCompiler.cs" company="">
//   
// </copyright>
// <summary>
//   The db command compiler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.Query
{
    /// <summary>
    ///     The db command compiler.
    /// </summary>
    internal class DbCommandCompiler
    {
        #region Fields

        /// <summary>
        ///     The _command.
        /// </summary>
        private readonly IDbCommand _command;

        /// <summary>
        ///     The _query.
        /// </summary>
        private readonly IArgumentQuery _query;

        /// <summary>
        ///     The _sql dialect.
        /// </summary>
        private readonly ISqlDialect _sqlDialect;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbCommandCompiler" /> class.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        internal DbCommandCompiler(IArgumentQuery query, ISqlDialect sqlDialect)
        {
            _query = query;
            _sqlDialect = sqlDialect;
            _command = sqlDialect.DbProvider.CreateCommand();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The compile.
        /// </summary>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        internal IDbCommand Compile()
        {
            SetupParameter();

            _command.CommandText = _query.SqlStatement;

            return _command;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The setup parameter.
        /// </summary>
        private void SetupParameter()
        {
            if (_query.Arguments == null)
            {
                return;
            }

            foreach (QueryParameter argument in _query.Arguments)
            {
                IDbDataParameter parameter = _command.CreateParameter();

                parameter.Setup(argument, _sqlDialect.SqlCharacters.ParameterPrefix);

                _command.Parameters.Add(parameter);
            }
        }

        #endregion
    }
}