using System;
using System.Collections.Generic;
using System.Linq;
using MicroORM.Mapping;
using MicroORM.Schema;
using MicroORM.Utils;

namespace MicroORM.Entity
{
    internal class EntityValueComparer
    {
        internal static KeyValuePair<string, object>[] ComputeChangedValues<TEntity>(KeyValuePair<string, object>[] namedValues, EntityValueCollection entityValueCollection)
        {
            namedValues = CleanUpNamedValues<TEntity>(namedValues);
            if (entityValueCollection.Count != 0 && (namedValues.Length != entityValueCollection.Count))
                return null;

            Dictionary<string, object> valuesForUpdate = new Dictionary<string, object>();
            for (int index = 0; index < namedValues.Length; index++)
            {
                string propertyName = namedValues[index].Key;
                KeyValuePair<string, object> keyValuePair = entityValueCollection.FirstOrDefault(kvp => kvp.Key == propertyName);
                if (keyValuePair.IsDefault() && entityValueCollection.Count != 0) continue; // Ja die Überprüfung auf Count != 0 ist vll. doof, aber dient dem Zweck!!

                if (NullValueCheck(namedValues[index].Value, keyValuePair.Value, valuesForUpdate, propertyName))
                    continue;
                else if (EnumValueCheck(namedValues[index].Value, keyValuePair.Value, valuesForUpdate, propertyName))
                    continue;
                else if (keyValuePair.Value.Equals(namedValues[index].Value) == false)
                    valuesForUpdate.Add(propertyName, namedValues[index].Value);
            }

            return valuesForUpdate.ToArray();
        }

        private static bool EnumValueCheck(object namedValue, object keyValue, Dictionary<string, object> valuesForUpdate, string propertyName)
        {
            if (namedValue == null)
                return false;

            Type enumType = namedValue.GetType();
            if (enumType.IsEnum)
            {
                int enumValue = Convert.ToInt32(namedValue);
                int keyIntValue = Convert.ToInt32(keyValue);
                if (enumValue != keyIntValue)
                    valuesForUpdate.Add(propertyName, enumValue);

                return true;
            }
            return false;
        }

        private static bool NullValueCheck(
            object namedValue,
            object keyValue,
            Dictionary<string, object> valuesForUpdate,
            string propertyName)
        {
            if ((namedValue == null && keyValue != null)
                || namedValue != null && keyValue == null)
            {
                valuesForUpdate.Add(propertyName, namedValue ?? keyValue);
                return true;
            }
            else if (namedValue == null && keyValue == null)
                return true;

            return false;
        }

        private static KeyValuePair<string, object>[] CleanUpNamedValues<TEntity>(KeyValuePair<string, object>[] namedValues)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            Dictionary<string, object> _validValues = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in namedValues)
            {
                foreach (DbColumn dbColumn in tableInfo.DbTable.DbColumns)
                {
                    if (kvp.Key == dbColumn.Name)
                    {
                        _validValues.Add(kvp.Key, kvp.Value);
                        break;
                    }
                }
            }
            return _validValues.ToArray();
        }
    }
}
