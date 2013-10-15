using System;
using System.Collections;
using System.Collections.Generic;

namespace MicroORM.Mapping
{
    internal sealed class MemberInfoCollection : IEnumerable<IMemberInfo>
    {
        private Dictionary<string, IMemberInfo> _membernameMemberMapping = new Dictionary<string, IMemberInfo>();
        private List<IMemberInfo> _memberInfos = new List<IMemberInfo>();

        internal void Add(IMemberInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            if (!_membernameMemberMapping.ContainsKey(info.Name))
            {
                _membernameMemberMapping.Add(info.Name, info);
                _memberInfos.Add(info);
            }
        }

        internal int Count { get { return _memberInfos.Count; } }

        internal IMemberInfo this[int index]
        {
            get { return _memberInfos[index]; }
        }

        public IEnumerator GetEnumerator()
        {
            return _memberInfos.GetEnumerator();
        }

        IEnumerator<IMemberInfo> IEnumerable<IMemberInfo>.GetEnumerator()
        {
            return _memberInfos.GetEnumerator();
        }
    }
}
