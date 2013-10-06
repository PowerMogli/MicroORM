using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Base.Mapping
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FieldAttribute : Attribute
    {
        private string _fieldName;
        private bool _isNullable = true;
        private bool _identifier;
        private bool _autoNumber;
        private int _size = -1;

        /// <summary>
        /// Creates a new instance of the <see cref="FieldAttribute">FieldAttribute
        /// Class</see>.
        /// </summary>
        public FieldAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FieldAttribute">FieldAttribute
        /// Class</see>.
        /// </summary>
        /// <param name="fieldName">Name of the field in the storage that is mapped to the property.</param>
        /// <exception cref="ArgumentNullException">FieldName is null.</exception>
        public FieldAttribute(string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");
            _fieldName = fieldName;
        }

        /// <summary>
        /// Returns the name of the field in the storage that is mapped to the
        /// property.
        /// </summary>
        /// <value>
        /// A string that represents the field in the storage connected with the
        /// property.
        /// </value>
        public string FieldName
        {
            get { return _fieldName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _fieldName = value;
            }
        }

        /// <summary>
        /// Specifies whether the field connected to the property is an identifier (primary
        /// key for some storage) of the entity.
        /// </summary>
        /// <value>Returns true if the field connected to the property is an identifier.</value>
        public bool Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        /// <summary>
        /// Specifies whether the field connected with the property is allowed to be DBNull
        /// in the storage.
        /// </summary>
        /// <remarks>
        /// The default behaviour is true, since usually properties are allowed to be DBNull.
        /// Set it to false to avoid saving of DBNull values (such properties are sometimes also
        /// named mandatory properties)
        /// </remarks>
        /// <value>Returns true if DBNull is allowed to be saved in the field.</value>
        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

        /// <summary>
        /// 	<para>Specifies whether the connected field of property is an auto number field in
        ///     the storage.</para>
        /// </summary>
        /// <remarks>
        /// 	<para>Auto number fields are used by a storage to create unique keys for objects in
        ///     the storage.</para>
        /// 	<para><b>Attention:</b> Currently auto numbers are only supported for
        ///     <see cref="Opf3.Storages.OleDb.AccessStorage">AccessStorage</see> and
        ///     <see cref="Opf3.Storages.MsSql.MsSqlStorage">MsSqlStorage</see>.</para>
        /// </remarks>
        /// <value>Returns true if the connected field is an auto number field.</value>
        public bool AutoNumber
        {
            get { return _autoNumber; }
            set { _autoNumber = value; }
        }

        /// <summary>
        /// Gets and sets the maximum length for fields in the storage that hold strings.
        /// </summary>
        public int Size
        {
            get { return _size; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value is below zero. Not allowed.");

                _size = value;
            }
        }
    }
}
