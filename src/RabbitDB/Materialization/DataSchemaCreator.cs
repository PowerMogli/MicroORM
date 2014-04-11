// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSchemaCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The data schema creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Materialization
{
    using System.Data;

    using RabbitDB.Mapping;

    /// <summary>
    /// The data schema creator.
    /// </summary>
    class DataSchemaCreator : IDataSchemaCreator
    {
        #region Fields

        /// <summary>
        /// The _column indexes.
        /// </summary>
        private int[] _columnIndexes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSchemaCreator"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
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

        #region Public Methods and Operators

        /// <summary>
        /// The column index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ColumnIndex(int index)
        {
            return _columnIndexes[index] - 1;
        }

        /// <summary>
        /// The create from type.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        public void CreateFromType(IDataReader dataReader, TableInfo tableInfo)
        {
            var membersCount = tableInfo.Columns.Count;

            _columnIndexes = new int[membersCount];
            var lowerNames = MemberFieldNameToLowers(tableInfo, membersCount);

            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                var columnName = dataReader.GetName(i).ToLower();

                for (var j = 0; j < tableInfo.Columns.Count; j++)
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

        #region Methods

        /// <summary>
        /// The member field name to lowers.
        /// </summary>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <param name="membersCount">
        /// The members count.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] MemberFieldNameToLowers(TableInfo tableInfo, int membersCount)
        {
            var lowerNames = new string[membersCount];
            for (var i = 0; i < membersCount; i++)
            {
                lowerNames[i] = tableInfo.Columns[i].ColumnAttribute.ColumnName.ToLower();
            }

            return lowerNames;
        }

        #endregion
    }
}