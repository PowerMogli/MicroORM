using RabbitDB.Mapping;
using RabbitDB.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Utils
{
    internal class ValidEntityArgumentReader<TEntity> : IValidEntityArgumentsReader
    {
        private TEntity _entity;

        internal ValidEntityArgumentReader(TEntity entity)
        {
            _entity = entity;
        }

        public IEnumerable<KeyValuePair<string, object>> ReadValidEntityArguments()
        {
            var entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { _entity });

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return entityValues.Where(kvp => tableInfo.DbTable.DbColumns.Any(column => column.Name == tableInfo.ResolveColumnName(kvp.Key)));
        }        

        //private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        ///// <summary>
        ///// Gets whether the given type is an anonymous type.
        ///// </summary>
        ///// <param name="type">The type that is inspected for being anonymous.</param>
        //internal static bool CheckIfAnonymousType(Type type)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException("type");

        //    // HACK: The only way to detect anonymous types right now.
        //    return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
        //           && type.IsGenericType && type.Name.Contains("AnonymousType")
        //           && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
        //           && (type.Attributes & _nonPublic) == _nonPublic;
        //}
    }
}