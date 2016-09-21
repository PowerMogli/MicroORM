// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSchemaCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The data schema creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Materialization;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.Materialization
{
    /// <summary>
    ///     The data schema creator.
    /// </summary>
    internal class DataSchemaCreator : IDataSchemaCreator
    {
        #region Fields

        /// <summary>
        ///     The _column indexes.
        /// </summary>
        private int[] _columnIndexes;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataSchemaCreator" /> class.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        internal DataSchemaCreator(IDataReader dataReader, TableInfo tableInfo)
        {
            if (tableInfo == null)
            {
                return;
            }

            CreateFromType(dataReader, tableInfo);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The column index.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int ColumnIndex(int index)
        {
            return _columnIndexes[index] - 1;
        }

        /// <summary>
        ///     The create from type.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        public void CreateFromType(IDataReader dataReader, ITableInfo tableInfo)
        {
            int membersCount = tableInfo.Columns.Count;

            _columnIndexes = new int[membersCount];
            string[] lowerNames = MemberFieldNameToLowers(tableInfo, membersCount);

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                string columnName = dataReader.GetName(i)
                                           .ToLower();

                for (int j = 0; j < tableInfo.Columns.Count; j++)
                {
                    if (lowerNames[j] != columnName)
                    {
                        continue;
                    }

                    _columnIndexes[j] = i + 1;
                    break;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The member field name to lowers.
        /// </summary>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <param name="membersCount">
        ///     The members count.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        private static string[] MemberFieldNameToLowers(ITableInfo tableInfo, int membersCount)
        {
            string[] lowerNames = new string[membersCount];

            for (int i = 0; i < membersCount; i++)
            {
                lowerNames[i] = tableInfo.Columns[i].ColumnAttribute.ColumnName.ToLower();
            }

            return lowerNames;
        }

        #endregion
    }
}