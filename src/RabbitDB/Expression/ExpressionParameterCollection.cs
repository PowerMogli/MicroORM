using System.Collections.Generic;

namespace RabbitDB.Expressions
{
    internal class ExpressionParameterCollection
    {
        private readonly List<object> _params = new List<object>();

        internal object[] ToArray()
        {
            return _params.ToArray();
        }

        internal int NextIndex
        {
            get { return _params.Count; }
        }

        internal void Add(object value)
        {
            _params.Add(value);
        }
    }
}