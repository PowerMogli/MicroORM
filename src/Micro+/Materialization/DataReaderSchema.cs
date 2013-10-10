using System;
using System.Data;
using MicroORM.Mapping;

namespace MicroORM.Materialization
{
    class DataReaderSchema
    {
        private int[] _columnIndexes;

        internal DataReaderSchema(IDataReader dataReader, Type entityType)
        {
            TableInfo tableInfo = TableInfo.GetTableInfo(entityType);
            if (tableInfo.PersistentAttribute == null)
                CreateDefaultIndexes(dataReader);
            else
                CreateFromType(dataReader, tableInfo);
        }

        private void CreateFromType(IDataReader dataReader, TableInfo tableInfo)
        {
            int membersCount = tableInfo.Members.Count;

            _columnIndexes = new int[membersCount];
            string[] _lowerNames = MemberFieldNameToLowers(tableInfo, membersCount);

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                string columnName = dataReader.GetName(i).ToLower();

                for (int j = 0; j < tableInfo.Members.Count; j++)
                {
                    if (_lowerNames[j] != columnName) continue;

                    _columnIndexes[j] = i + 1;
                    break;
                }
            }
        }

        private void CreateDefaultIndexes(IDataReader dataReader)
        {
            _columnIndexes = new int[dataReader.FieldCount];
            for (int index = 0; index < dataReader.FieldCount; index++)
            {
                _columnIndexes[index] = index++;
            }
        }

        string[] MemberFieldNameToLowers(TableInfo tableInfo, int membersCount)
        {
            string[] lowerNames = new string[membersCount];
            for (int i = 0; i < membersCount; i++)
            {
                lowerNames[i] = tableInfo.Members[i].FieldAttribute.FieldName.ToLower();
            }
            return lowerNames;
        }

        internal int ColumnIndex(int index)
        {
            return _columnIndexes[index] - 1;
        }
    }
}

