using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Mapping
{
    internal sealed class PropertyInfoCollection : IEnumerable<IPropertyInfo>
    {
        private Dictionary<string, IPropertyInfo> _propertyNameMemberMapping = new Dictionary<string, IPropertyInfo>();
        private List<IPropertyInfo> _propertyInfos = new List<IPropertyInfo>();

        internal void Add(IPropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!_propertyNameMemberMapping.ContainsKey(propertyInfo.Name))
            {
                _propertyNameMemberMapping.Add(propertyInfo.Name, propertyInfo);
                _propertyInfos.Add(propertyInfo);
            }
        }

        internal int Count { get { return _propertyInfos.Count; } }

        internal IPropertyInfo this[int index]
        {
            get { return _propertyInfos[index]; }
        }

        internal void Remove(string name)
        {
            IPropertyInfo propertyInfo = _propertyNameMemberMapping[name];
            _propertyNameMemberMapping.Remove(name);
            _propertyInfos.Remove(propertyInfo);
        }

        public IEnumerator GetEnumerator()
        {
            return _propertyInfos.GetEnumerator();
        }

        IEnumerator<IPropertyInfo> IEnumerable<IPropertyInfo>.GetEnumerator()
        {
            return _propertyInfos.GetEnumerator();
        }

        public bool Contains(string columnName)
        {
            return _propertyInfos.Any(propertyInfo => propertyInfo.ColumnAttribute.ColumnName == columnName);
        }
    }
}
