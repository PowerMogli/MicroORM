using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MicroORM.Base;
using MicroORM.Storage;
using MicroORM.Attributes;
using System.Collections.Generic;

namespace MicroORM.Mapping
{
    internal sealed class TableInfo
    {
        private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        private bool _isAnonymousType;
        private string _selectStatement;
        private string _insertStatement;
        private string[] _primaryKeys;

        private string SelectStatement
        {
            get { return _selectStatement; }
            set { _selectStatement = value; }
        }

        private string InsertStatement
        {
            get { return _insertStatement; }
            set { _insertStatement = value; }
        }

        internal int NumberOfPrimaryKeys
        {
            get
            {
                if (_primaryKeys == null)
                    _primaryKeys = GetPrimaryKeys();
                if (_primaryKeys == null) return 0;
                return _primaryKeys.Length;
            }
        }


        /// <summary>
        /// Creates a new instance of the <see cref="TableInfo">TypeMapping Class</see>.
        /// </summary>
        /// <param name="type">Type of object managed by this class.</param>
        /// <param name="persistentAttribute">Persistent attribute that decorates the class.</param>
        /// <param name="members">The list of the mapped members.</param>
        internal TableInfo(Type type, string tableName)
        {
            this.EntityType = type;
            this.Name = tableName;
            this.Columns = new PropertyInfoCollection();
            this.PrimaryKeys = new PropertyInfoCollection();

            // Check if we have to deal with an anonymous type.
            _isAnonymousType = CheckIfAnonymousType(type);
        }

        internal string CreateSelectStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false) return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("select ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Columns.Select(member => provider.EscapeName(member.Name))));
            selectStatement.AppendFormat(" from {0}", provider.EscapeName(this.Name));
            string[] primaryKeys = GetPrimaryKeys();

            selectStatement.Append(AppendPrimaryKeys(provider, primaryKeys));
            return this.SelectStatement = selectStatement.ToString();
        }

        private string AppendPrimaryKeys(IDbProvider provider, string[] primaryKeys)
        {
            StringBuilder whereClause = new StringBuilder(" where ");
            int i = 0;
            string seperator = " and ";
            foreach (string pirmaryKey in primaryKeys)
            {
                if (i >= primaryKeys.Length - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", provider.EscapeName(pirmaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }

        internal string CreateInsertStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.InsertStatement) == false) return this.InsertStatement;

            StringBuilder insertStatement = new StringBuilder();
            insertStatement.AppendFormat("insert into {0} ", provider.EscapeName(this.Name));

            var tuple = CreateInsertIntoValues(provider);
            insertStatement.AppendFormat("({0})", tuple.Item1);
            insertStatement.AppendFormat(" values(@{0})", string.Join(",@", Enumerable.Range(0, this.Columns.Count - tuple.Item2)));

            return this.InsertStatement = insertStatement.ToString();
        }

        private Tuple<string, int> CreateInsertIntoValues(IDbProvider provider)
        {
            int excludedValues = 0;
            string insertValues = string.Join(", ", this.Columns.Select(member =>
            {
                if (this.PrimaryKeys.Contains(member.Name)
                    && ((ColumnAttribute)(member.ColumnAttribute)).AutoNumber)
                {
                    excludedValues++;
                    return string.Empty;
                }
                return provider.EscapeName(member.Name);
            }));

            if (insertValues.StartsWith(","))
                insertValues = insertValues.Substring(2, insertValues.Length - 2);

            return new Tuple<string, int>(insertValues, excludedValues);
        }

        /// <summary>
        /// Gets whether the given type is an anonymous type.
        /// </summary>
        /// <param name="type">The type that is inspected for being anonymous.</param>
        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & _nonPublic) == _nonPublic;
        }

        internal object[] GetPrimaryKeys<TEntity>(TEntity entity)
        {
            object[] primaryKeys = null;
            string[] tablePrimaryKeys = GetPrimaryKeys();
            if (tablePrimaryKeys.Length <= 0) throw new PrimaryKeyException("It was no valid primaryKey available!");

            primaryKeys = new object[tablePrimaryKeys.Length];
            int index = 0;
            foreach (string tablePrimaryKey in tablePrimaryKeys)
            {
                object primaryKey = null;
                IPropertyInfo memberInfo = this.Columns.FirstOrDefault(member => member.Name == tablePrimaryKey);

                if (memberInfo == null || (primaryKey = memberInfo.GetValue(entity)) == null)
                    throw new PrimaryKeyException(string.Format("The requested pirmaryKey '{0}' was not found! Please set those Property to a valid value!", tablePrimaryKey));

                primaryKeys[index++] = primaryKey;
            }
            return primaryKeys;
        }

        private string[] GetPrimaryKeys()
        {
            List<string> tablePrimaryKeys = this.PrimaryKeys.Select(member => member.Name).ToList();
            tablePrimaryKeys.ForEach(pk => pk = pk.Trim());

            return tablePrimaryKeys.ToArray();
        }

        public string Name { get; private set; }

        /// <summary>
        /// Returns the type of the mapping's persistent object.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets whether the type is an anonymous type.
        /// </summary>
        public bool IsAnonymousType
        {
            get { return _isAnonymousType; }
        }

        /// <summary>
        /// Returns the collection of members associated with the <see cref="TableInfo">TypeMapping</see>.
        /// </summary>
        internal PropertyInfoCollection Columns { get; private set; }

        internal PropertyInfoCollection PrimaryKeys { get; set; }

        /// <summary>
        /// Returns the mapping for a given object.
        /// </summary>
        /// <param name="obj">The object the mapping is returned for.</param>
        public static TableInfo GetTableInfo(object obj)
        {
            return TableInfoContainer.GetTableInfo(obj);
        }

        /// <summary>
        /// Returns the mapping for the given persistent type.
        /// </summary>
        /// <param name="type">Type of object the mapping is returned for.</param>
        public static TableInfo GetTableInfo(Type type)
        {
            return TableInfoContainer.GetTableInfo(type);
        }
    }
}
