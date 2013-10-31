using System.Collections.Generic;

namespace RabbitDB.Expressions
{
    public class ExpressionParameterCollection
    {
        private readonly List<object> _params = new List<object>();

        public object[] ToArray()
        {
            return _params.ToArray();
        }

        public int NextIndex
        {
            get { return _params.Count; }
        }

        public void Add(object value)
        {
            _params.Add(value);
        }
    }
}