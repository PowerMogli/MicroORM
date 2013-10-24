namespace MicroORM.Schema
{
    internal class DbForeignKey
    {
        public string ToTable { get; set; }
        public string FromColumn { get; set; }
        public string ToColumn { get; set; }
    }
}
