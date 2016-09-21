#region using directives

using System.Collections.Generic;
using System.Data;

#endregion

namespace RabbitDB.Contracts.Query.StoredProcedure
{
    public interface IProcedureParameterCollection : IEnumerable<IDbDataParameter>
    {
        #region  Properties

        IDbDataParameter this[string key] { get; }

        #endregion

        #region Public Methods

        void Add(string key, IDbDataParameter parameter);

        bool ContainsKey(string key);

        T GetParameterValue<T>(string parameterName);

        bool IsParameterValid<T>(string parameterName, T value, DbType dbType, int length);

        #endregion
    }
}