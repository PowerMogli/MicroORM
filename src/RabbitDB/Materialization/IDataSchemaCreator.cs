using System.Data;
using RabbitDB.Mapping;

namespace RabbitDB.Materialization
{
    interface IDataSchemaCreator
    {
        void CreateFromType(IDataReader dataReader, TableInfo tableInfo);
        int ColumnIndex(int index);
    }
}
