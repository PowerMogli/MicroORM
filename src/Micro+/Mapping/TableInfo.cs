using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MicroORM.Base;
using MicroORM.Storage;

namespace MicroORM.Mapping
{
    internal sealed class TableInfo
    {
        private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        private Type _type;
        private TableAttribute _persistentAttribute;
        private MemberInfoCollection _members;
        private bool _isAnonymousType;
        private string _selectStatement;
        private string _insertStatement;

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
                string[] primaryKeys = GetPrimaryKeys();
                if (primaryKeys == null) return 0;
                return primaryKeys.Length;
            }
        }

        internal string CreateSelectStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false) return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("select ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Members.Select(member => provider.EscapeName(member.FieldAttribute.FieldName))));
            selectStatement.AppendFormat(" from {0}", provider.EscapeName(PersistentAttribute.EntityName));
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
            insertStatement.AppendFormat("insert into {0} ", provider.EscapeName(this.PersistentAttribute.EntityName));

            var tuple = CreateInsertIntoValues(provider);
            insertStatement.AppendFormat("({0})", tuple.Item1);
            insertStatement.AppendFormat(" values(@{0})", string.Join(",@", Enumerable.Range(0, this.Members.Count - tuple.Item2)));

            return this.InsertStatement = insertStatement.ToString();
        }

        private Tuple<string, int> CreateInsertIntoValues(IDbProvider provider)
        {
            int excludedValues = 0;
            string insertValues = string.Join(", ", this.Members.Select(member =>
            {
                if (this.PersistentAttribute.PrimaryKeys.Contains(member.FieldAttribute.FieldName)
                    && member.FieldAttribute.AutoNumber)
                {
                    excludedValues++;
                    return string.Empty;
                }
                return provider.EscapeName(member.FieldAttribute.FieldName);
            }));

            if (insertValues.StartsWith(","))
                insertValues = insertValues.Substring(2, insertValues.Length - 2);

            return new Tuple<string, int>(insertValues, excludedValues);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TableInfo">TypeMapping Class</see>.
        /// </summary>
        /// <param name="type">Type of object managed by this class.</param>
        /// <param name="persistentAttribute">Persistent attribute that decorates the class.</param>
        /// <param name="members">The list of the mapped members.</param>
        internal TableInfo(Type type, TableAttribute persistentAttribute, MemberInfoCollection members)
        {
            //if (persistentAttribute == null)
            //    throw new ArgumentNullException("persistentAttribute", "Can't create persistent mapping without persistent attribute.");

            _type = type;
            _persistentAttribute = persistentAttribute;
            _members = members;
            // Check if we have to deal with an anonymous type.
            _isAnonymousType = CheckIfAnonymousType(type);
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
                IMemberInfo memberInfo = this.Members.FirstOrDefault(member => member.FieldAttribute.FieldName == tablePrimaryKey);

                if (memberInfo == null || (primaryKey = memberInfo.GetValue(entity)) == null)
                    throw new PrimaryKeyException(string.Format("The requested pirmaryKey '{0}' was not found! Please set those Property to a valid value!", tablePrimaryKey));

                primaryKeys[index++] = primaryKey;
            }
            return primaryKeys;
        }

        private string[] GetPrimaryKeys()
        {
            string[] tablePrimaryKeys = this.PersistentAttribute.PrimaryKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tablePrimaryKeys.Length; i++)
            {
                tablePrimaryKeys[i] = tablePrimaryKeys[i].Trim();
            }

            return tablePrimaryKeys;
        }

        internal object GetPrimaryKey<TEntity>(TEntity entity)
        {
            string tablePrimaryKey = this.PersistentAttribute.PrimaryKeys;
            IMemberInfo memberInfo = this.Members.FirstOrDefault(member => member.FieldAttribute.FieldName == tablePrimaryKey);

            object primaryKey = null;
            if (memberInfo == null || (primaryKey = memberInfo.GetValue(entity)) == null) throw new PrimaryKeyException("Es wurde kein gültiger PrimaryKey gesetzt!");

            return primaryKey;
        }
        /// <summary>
        /// Returns the persistent attribute associated with the object.
        /// </summary>
        public TableAttribute PersistentAttribute
        {
            get { return _persistentAttribute; }
        }

        /// <summary>
        /// Returns the type of the mapping's persistent object.
        /// </summary>
        public Type PersistentType
        {
            get { return _type; }
        }

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
        internal MemberInfoCollection Members
        {
            get { return _members; }
        }

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
