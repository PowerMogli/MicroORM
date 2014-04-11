// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidEntityArgumentReader.cs" company="">
//   
// </copyright>
// <summary>
//   The valid entity argument reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.Mapping;
    using RabbitDB.Reflection;

    /// <summary>
    /// The valid entity argument reader.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class ValidEntityArgumentReader<TEntity> : IValidEntityArgumentsReader
    {
        #region Fields

        /// <summary>
        /// The _entity.
        /// </summary>
        private readonly TEntity entity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidEntityArgumentReader{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal ValidEntityArgumentReader(TEntity entity)
        {
            this.entity = entity;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The read valid entity arguments.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<KeyValuePair<string, object>> ReadValidEntityArguments()
        {
            var entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { this.entity });

            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            return
                entityValues.Where(
                    kvp =>
                    tableInfo.DbTable.DbColumns.Any(column => column.Name == tableInfo.ResolveColumnName(kvp.Key)));
        }

        #endregion

        // private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        ///// <summary>
        ///// Gets whether the given type is an anonymous type.
        ///// </summary>
        ///// <param name="type">The type that is inspected for being anonymous.</param>
        // internal static bool CheckIfAnonymousType(Type type)
        // {
        // if (type == null)
        // throw new ArgumentNullException("type");

        // // HACK: The only way to detect anonymous types right now.
        // return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
        // && type.IsGenericType && type.Name.Contains("AnonymousType")
        // && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
        // && (type.Attributes & _nonPublic) == _nonPublic;
        // }
    }
}