using System;
using System.Data;
using RabbitDB.Attributes;
using RabbitDB.Base;
using RabbitDB.Query;
using RabbitDB.Schema;
using RabbitDB.Storage;

namespace RabbitDB.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Registrar<DbEngine>.Register("RabbitDB.Program.*", DbEngine.SqlServer);
                Registrar<string>.Register("RabbitDB.Program.*", @"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True");

                using (DbSession dbSession = new DbSession(typeof(Program)))
                {
                    string sql = @"select * from Posts;
                                   select * from Users;";

                    var multiset = dbSession.ExecuteMultiple(sql);
                    var posts = multiset.Read<Post>();
                    var users = multiset.Read<Users>();
                }
                DbSchemaAllocator.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    class Person
    {
        public string Name { get; set; }
        public Users User { get; set; }
    }

    class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ImportPrepareProcedureObject : SqlStoredProcedure
    {
        public ImportPrepareProcedureObject()
            : base("spExecuteSomething") { }

        public string TicketID
        {
            get { return base.GetParameterValue<string>("@pTicketID"); }
            set { base.AddParameter("@pTicketID", value, DbType.AnsiString, 255); }
        }
    }

    [Table("Posts")]
    class Post : Entity.Entity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public PostType Type { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }
        public string Test1 { get; set; }
        public bool Test2 { get; set; }
    }

    public enum PostType
    {
        Post,
        Page
    }
}