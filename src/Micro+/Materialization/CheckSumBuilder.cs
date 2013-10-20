using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Globalization;
using MicroORM.Mapping;

namespace MicroORM.Materialization
{
    internal class CheckSumBuilder
    {
        private ByteArrayBuilder _byteArrayBuilder = new ByteArrayBuilder();
        CultureInfo _germanCulture = new CultureInfo("de-DE");

        internal byte[] ToCheckSum()
        {
            byte[] byteSum = _byteArrayBuilder.ToByteArray();
            if (byteSum == null) return null;

            return new MD5CryptoServiceProvider().ComputeHash(byteSum);
        }

        internal void AddPropertyValue(object value)
        {
            if (value == null)
                return;

            string valueStr = value as string;
            if (string.IsNullOrWhiteSpace(valueStr) == false)
            {
                _byteArrayBuilder.Append(valueStr);
                return;
            }

            if (value is int)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((int)value));
                return;
            }

            if (value is byte)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((byte)value));
                return;
            }

            if (value is short)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((short)value));
                return;
            }

            if (value is long)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((long)value));
                return;
            }

            if (value is decimal)
            {
                _byteArrayBuilder.Append(((decimal)value).ToString(_germanCulture));
                return;
            }

            if (value is double)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((double)value));
                return;
            }

            if (value is DateTime)
            {
                _byteArrayBuilder.Append(((DateTime)value).ToString("D", _germanCulture));
                return;
            }

            if (value is bool)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((bool)value));
                return;
            }

            if (value is Single)
            {
                _byteArrayBuilder.Append(BitConverter.GetBytes((Single)value));
                return;
            }

            byte[] _bytes = value as byte[];
            if (_bytes != null)
            {
                _byteArrayBuilder.Append(_bytes);
                return;
            }

            _byteArrayBuilder.Append(value.ToString());
        }

        internal static byte[] CalculateEntityChecksum(object entity)
        {
            CheckSumBuilder checkSumBuilder = new CheckSumBuilder();

            TableInfo typeMapping = TableInfo.GetTableInfo(entity);

            foreach (IPropertyInfo propertyInfo in typeMapping.Columns)
            {
                checkSumBuilder.AddPropertyValue(propertyInfo.GetValue(entity));
            }

            return checkSumBuilder.ToCheckSum();
        }
    }
}
