
namespace MicroORM.Query
{
    public class LastInsertId
    {
        private object _value;

        internal LastInsertId(object value)
        {
            _value = value;
        }

        public bool IsEmpty { get { return _value == null; } }

        public T ConvertTo<T>()
        {
            return (T)_value;
        }
    }
}
