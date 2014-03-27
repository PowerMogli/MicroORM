using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RabbitDB.Reflection
{
    internal class ParameterTypeDescriptor
    {
        internal static KeyValuePair<string, object>[] ToKeyValuePairs(object[] arguments)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (arguments != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(arguments[0]))
                {
                    result.Add(property.Name, property.GetValue(arguments[0]));
                }
            }
            return result.ToArray();
        }
    }
}
