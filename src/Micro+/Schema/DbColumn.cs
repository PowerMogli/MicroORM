using System.Data;

namespace MicroORM.Schema
{
    internal class DbColumn
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public DbType DbType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsComputed { get; set; }
        public bool Ignore { get; set; }
        public int Size { get; set; }
        public int Precision { get; set; }
        public string DefaultValue { get; set; }
    }
}
