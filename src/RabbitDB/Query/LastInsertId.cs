namespace RabbitDB.Query
{
    public class LastInsertId
    {
        private object _value;

        internal LastInsertId(object value)
        {
            _value = value;
        }

        internal static implicit operator object(LastInsertId lastInsertID)
        {
            return lastInsertID._value;
        }

        public bool IsEmpty { get { return _value == null; } }
    }
}
