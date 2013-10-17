using System.Data;
namespace MicroORM.Query
{
    public abstract class StoredProcedure
    {
        internal StoredProcedure(string storedProcedureName)
        {
            this.StoredProcedureName = storedProcedureName;
            this.Parameters = new ProcedureParameterCollection();
        }

        internal string StoredProcedureName { get; private set; }
        internal ProcedureParameterCollection Parameters { get; private set; }

        protected abstract bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1);

        protected abstract T GetParameterValue<T>(string parameterName);
    }
}
