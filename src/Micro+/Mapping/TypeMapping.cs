using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MicroORM.Base.Storage;

namespace MicroORM.Base.Mapping
{
    internal sealed class TypeMapping
    {
        private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        private Type _type;
        private TableAttribute _persistentAttribute;
        private MemberInfoCollection _members;
        private bool _isAnonymousType;
        private string _selectStatement;

        private string SelectStatement
        {
            get { return _selectStatement; }
            set { _selectStatement = value; }
        }

        internal string CreateSelectStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false) return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("select ");

            string seperator = ", ";
            for (int index = 0; index < _members.Count; index++)
            {
                if (index >= _members.Count - 1) seperator = "";
                selectStatement.AppendFormat("{0}{1}", provider.EscapeName(_members[index].FieldAttribute.FieldName), seperator);
            }
            selectStatement.AppendFormat(" from {0}", provider.EscapeName(PersistentAttribute.EntityName));
            selectStatement.AppendFormat(" where {0}=@0", provider.EscapeName(PersistentAttribute.PrimaryKey));

            return this.SelectStatement = selectStatement.ToString();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeMapping">TypeMapping Class</see>.
        /// </summary>
        /// <param name="type">Type of object managed by this class.</param>
        /// <param name="persistentAttribute">Persistent attribute that decorates the class.</param>
        /// <param name="members">The list of the mapped members.</param>
        internal TypeMapping(Type type, TableAttribute persistentAttribute, MemberInfoCollection members)
        {
            if (persistentAttribute == null)
                throw new ArgumentNullException("persistentAttribute", "Can't create persistent mapping without persistent attribute.");

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
        /// Returns the collection of members associated with the <see cref="TypeMapping">TypeMapping</see>.
        /// </summary>
        internal MemberInfoCollection Members
        {
            get { return _members; }
        }

        /// <summary>
        /// Returns the mapping for a given object.
        /// </summary>
        /// <param name="obj">The object the mapping is returned for.</param>
        public static TypeMapping GetTypeMapping(object obj)
        {
            return TypeMappingContainer.GetTypeMapping(obj);
        }

        /// <summary>
        /// Returns the mapping for the given persistent type.
        /// </summary>
        /// <param name="type">Type of object the mapping is returned for.</param>
        public static TypeMapping GetTypeMapping(Type type)
        {
            return TypeMappingContainer.GetTypeMapping(type);
        }
    }
}
