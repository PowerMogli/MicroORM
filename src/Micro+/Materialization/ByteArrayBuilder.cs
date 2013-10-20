using System.Collections.Generic;
using System.Text;

namespace MicroORM.Materialization
{
    internal class ByteArrayBuilder
    {
        List<byte> _byteValues = new List<byte>();
        StringBuilder _stringValues = new StringBuilder();

        internal void Append(string value)
        {
            _stringValues.AppendFormat("|{0}|", value);
        }

        internal void Append(byte[] value)
        {
            _byteValues.AddRange(value);
        }

        internal byte[] ToByteArray()
        {
            _byteValues.AddRange(new UTF8Encoding().GetBytes(_stringValues.ToString()));
            return _byteValues.ToArray();
        }
    }
}
