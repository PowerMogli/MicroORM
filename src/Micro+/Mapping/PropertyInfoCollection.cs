using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MicroORM.Mapping
{
    internal sealed class PropertyInfoCollection : IEnumerable<IPropertyInfo>
    {
        private Dictionary<string, IPropertyInfo> _membernameMemberMapping = new Dictionary<string, IPropertyInfo>();
        private List<IPropertyInfo> _memberInfos = new List<IPropertyInfo>();

        internal void Add(IPropertyInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");

            if (!_membernameMemberMapping.ContainsKey(memberInfo.Name))
            {
                _membernameMemberMapping.Add(memberInfo.Name, memberInfo);
                _memberInfos.Add(memberInfo);
            }
        }

        internal int Count { get { return _memberInfos.Count; } }

        internal IPropertyInfo this[int index]
        {
            get { return _memberInfos[index]; }
        }

        public IEnumerator GetEnumerator()
        {
            return _memberInfos.GetEnumerator();
        }

        IEnumerator<IPropertyInfo> IEnumerable<IPropertyInfo>.GetEnumerator()
        {
            return _memberInfos.GetEnumerator();
        }

        public bool Contains(string columnName)
        {
            return _memberInfos.Any(member => member.ColumnAttribute.ColumnName == columnName);
        }
    }
}
