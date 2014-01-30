using System.Data;
using RabbitDB.Mapping;

namespace RabbitDB.Materialization
{
    class DataSchemaCreator : IDataSchemaCreator
    {
        private int[] _columnIndexes;

        internal DataSchemaCreator(IDataReader dataReader, TableInfo tableInfo)
        {
            if (tableInfo == null) return;

            CreateFromType(dataReader, tableInfo);
        }

        public void CreateFromType(IDataReader dataReader, TableInfo tableInfo)
        {
            int membersCount = tableInfo.Columns.Count;

            _columnIndexes = new int[membersCount];
            string[] _lowerNames = MemberFieldNameToLowers(tableInfo, membersCount);

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                string columnName = dataReader.GetName(i).ToLower();

                for (int j = 0; j < tableInfo.Columns.Count; j++)
                {
                    if (_lowerNames[j] != columnName) continue;

                    _columnIndexes[j] = i + 1;
                    break;
                }
            }
        }

        private string[] MemberFieldNameToLowers(TableInfo tableInfo, int membersCount)
        {
            string[] lowerNames = new string[membersCount];
            for (int i = 0; i < membersCount; i++)
            {
                lowerNames[i] = tableInfo.Columns[i].ColumnAttribute.ColumnName.ToLower();
            }
            return lowerNames;
        }

        public int ColumnIndex(int index)
        {
            return _columnIndexes[index] - 1;
        }
    }
}

