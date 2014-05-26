// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryParameter.cs" company="">
//   
// </copyright>
// <summary>
//   The query parameter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    using RabbitDB.Mapping;

    /// <summary>
    /// The query parameter.
    /// </summary>
    class QueryParameter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        internal QueryParameter(string name, DbType dbType, object value, int size = -1)
        {
            this.Name = name;
            this.DbType = dbType;
            this.Value = value;
            this.Size = size > 0 ? new int?(size) : null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the db type.
        /// </summary>
        internal DbType DbType { get; private set; }


        internal bool IsInvalid
        {
            get
            {
                return ((DbType == DbType.AnsiString
                    || DbType == DbType.AnsiStringFixedLength
                    || DbType == DbType.String
                    || DbType == DbType.StringFixedLength)
                    && Size > 0
                    && (Value is string && ((string)Value).Length > Size));
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        internal int? Size { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        internal object Value { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The create from db parameter.
        /// </summary>
        /// <param name="dbDataParameter">
        /// The db data parameter.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameter"/>.
        /// </returns>
        internal static QueryParameter CreateFromDbParameter(IDbDataParameter dbDataParameter)
        {
            return new QueryParameter(
                dbDataParameter.ParameterName,
                dbDataParameter.DbType,
                dbDataParameter.Value,
                dbDataParameter.Size);
        }

        /// <summary>
        /// The create from key value pairs.
        /// </summary>
        /// <param name="keyValuePair">
        /// The key value pair.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameter"/>.
        /// </returns>
        internal static QueryParameter CreateFromKeyValuePairs(
            KeyValuePair<string, object> keyValuePair,
            TableInfo tableInfo)
        {
            Type argumentType = null;
            if (keyValuePair.Value != null)
            {
                argumentType = keyValuePair.Value.GetType();
                if (argumentType.IsEnum)
                {
                    argumentType = typeof(Int32);
                }
            }

            var isColumn = tableInfo != null && tableInfo.IsColumn(keyValuePair.Key);

            return new QueryParameter(
                keyValuePair.Key,
                isColumn ? tableInfo.ConvertToDbType(keyValuePair.Key) : TypeConverter.ToDbType(argumentType),
                EvaluateParameterValue(tableInfo, keyValuePair),
                isColumn ? tableInfo.GetColumnSize(keyValuePair.Key) : -1);
        }

        /// <summary>
        /// The create from regular.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameter"/>.
        /// </returns>
        internal static QueryParameter CreateFromRegular(int index, object[] arguments)
        {
            var argumentType = arguments[index].GetType();
            if (argumentType.IsEnum)
            {
                argumentType = typeof(Int32);
            }

            return new QueryParameter(
                index.ToString(CultureInfo.InvariantCulture),
                TypeConverter.ToDbType(argumentType),
                arguments[index]);
        }

        /// <summary>
        /// The evaluate parameter value.
        /// </summary>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <param name="kvp">
        /// The kvp.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object EvaluateParameterValue(TableInfo tableInfo, KeyValuePair<string, object> kvp)
        {
            if (tableInfo == null)
            {
                return kvp.Value ?? DBNull.Value;
            }

            if (kvp.Value != null)
            {
                return kvp.Value;
            }

            var dbColumn =
                tableInfo.DbTable.DbColumns.FirstOrDefault(column => column.Name == kvp.Key && column.IsNullable);

            if (dbColumn == null)
            {
                return DBNull.Value;
            }

            if (string.IsNullOrWhiteSpace(dbColumn.DefaultValue))
            {
                return DBNull.Value;
            }

            if (dbColumn.DefaultValue == "getdate()" || dbColumn.DefaultValue == "(getdate())")
            {
                return DateTime.Now.ToShortDateString();
            }

            return dbColumn.DefaultValue == "newid()"
                       ? Guid.NewGuid().ToString()
                       : dbColumn.DefaultValue.Replace("(", string.Empty).Replace(")", string.Empty);
        }

        #endregion
    }
}