using System;
using System.Collections.Generic;
using System.Data;

namespace RabbitDB.Mapping
{
    internal static class TypeConverter
    {
        private static readonly Dictionary<Type, DbType> typeToDbType = 
            new Dictionary<Type, DbType>
            {
            { typeof(string), DbType.AnsiString },
            { typeof(char), DbType.AnsiStringFixedLength },
            { typeof(DateTime), DbType.DateTime },
            { typeof(DateTime?), DbType.DateTime },
            { typeof(int), DbType.Int32 },
            { typeof(int?), DbType.Int32 },
            { typeof(long), DbType.Int64 },
            { typeof(long?), DbType.Int64 },
            { typeof(bool), DbType.Boolean },
            { typeof(bool?), DbType.Boolean },
            { typeof(byte[]), DbType.Binary },
            { typeof(decimal), DbType.Decimal },
            { typeof(decimal?), DbType.Decimal },
            { typeof(double), DbType.Double },
            { typeof(double?), DbType.Double },
            { typeof(float), DbType.Single },
            { typeof(float?), DbType.Single },
            { typeof(Guid), DbType.Guid },
            { typeof(Guid?), DbType.Guid }};

        private static readonly Dictionary<string, DbType> stringToDbType =
            new Dictionary<string, DbType>
            {
                { "nvarchar", DbType.String },
                { "varchar", DbType.AnsiString },
                { "char", DbType.AnsiStringFixedLength },
                { "nchar", DbType.StringFixedLength },
                { "int", DbType.Int32 },
                { "bigint", DbType.Int64 },
                { "smallint", DbType.Int16 },
                { "datetime", DbType.DateTime },
                { "smalldatetime", DbType.DateTime },
                { "datetime2", DbType.DateTime2 },
                { "date", DbType.Date },
                { "time", DbType.Time },
                { "uniqueidentifier", DbType.Guid },
                { "datetimeoffset", DbType.DateTimeOffset },
                { "float", DbType.Double },
                { "real", DbType.Double },
                { "numeric", DbType.Decimal },
                { "smallmoney", DbType.Decimal },
                { "decimal", DbType.Decimal },
                { "money", DbType.Decimal },
                { "tinyint", DbType.Byte },
                { "bit", DbType.Boolean },
                { "image", DbType.Binary },
                { "binary", DbType.Binary },
                { "varbinary", DbType.Binary },
                { "timestamp", DbType.Binary },
            };


        internal static DbType ToDbType(string type)
        {
            if (!stringToDbType.ContainsKey(type))
                throw new InvalidOperationException(string.Format("Type {0} doesn´t have a matching DbType configured.", type));

            return stringToDbType[type];
        }

        internal static DbType ToDbType(Type type)
        {
            if (!typeToDbType.ContainsKey(type))
                throw new InvalidOperationException(string.Format("Type {0} doesn't have a matching DbType configured.", type.FullName));

            return typeToDbType[type];
        }
    }
}
