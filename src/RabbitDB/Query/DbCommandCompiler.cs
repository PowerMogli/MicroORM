// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbCommandCompiler.cs" company="">
//   
// </copyright>
// <summary>
//   The db command compiler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System.Data;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The db command compiler.
    /// </summary>
    internal class DbCommandCompiler
    {
        #region Fields

        /// <summary>
        /// The _command.
        /// </summary>
        private readonly IDbCommand _command;

        /// <summary>
        /// The _query.
        /// </summary>
        private readonly IArgumentQuery _query;

        /// <summary>
        /// The _sql dialect.
        /// </summary>
        private readonly SqlDialect _sqlDialect;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandCompiler"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal DbCommandCompiler(IArgumentQuery query, SqlDialect sqlDialect)
        {
            _query = query;
            _sqlDialect = sqlDialect;
            _command = sqlDialect.DbProvider.CreateCommand();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The compile.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        internal IDbCommand Compile()
        {
            SetupParameter();
            _command.CommandText = _query.SqlStatement;

            return _command;
        }

        /// <summary>
        /// The setup parameter.
        /// </summary>
        private void SetupParameter()
        {
            if (_query.Arguments == null)
            {
                return;
            }

            foreach (var argument in _query.Arguments)
            {
                var parameter = _command.CreateParameter();
                parameter.Setup(argument, _sqlDialect.SqlCharacters.ParameterPrefix);

                _command.Parameters.Add(parameter);
            }
        }

        #endregion
    }
}