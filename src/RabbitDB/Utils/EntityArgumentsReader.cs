using RabbitDB.Mapping;
using RabbitDB.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Utils
{
    internal class EntityArgumentsReader
    {
        internal object[] GetEntityArguments<TEntity>(TEntity entity, TableInfo tableInfo)
        {
            KeyValuePair<string, object>[] properties = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });
            int count = properties.Count();

            List<KeyValuePair<string, object>> arguments = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < count; i++)
            {
                IPropertyInfo propertyInfo = tableInfo.Columns.Where(column => column.ColumnAttribute.ColumnName == properties[i].Key).FirstOrDefault();
                if (propertyInfo == null
                    || (tableInfo.Columns.Contains(propertyInfo.ColumnAttribute.ColumnName)
                    && (propertyInfo.ColumnAttribute.AutoNumber || propertyInfo.ColumnAttribute.IsPrimaryKey)))
                {
                    continue;
                }

                arguments.Add(properties[i]);
            }
            return new object[] { arguments.ToArray() };
        }
    }
}