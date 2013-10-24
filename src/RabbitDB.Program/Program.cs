using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using MicroORM.Attributes;
using MicroORM.Base;
using MicroORM.Entity;
using MicroORM.Query;
using MicroORM.Schema;
using MicroORM.Storage;

namespace Micro.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Registrar<DbEngine>.Register("Micro.Program.*", DbEngine.SqlServer);
                Registrar<string>.Register("Micro.Program.*", @"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True");

                using (DbSession dbSession = new DbSession(typeof(Program)))
                {
                    //session.ExecuteCommand("select * from Posts where Title=@0 and Id=@1", "Mark", 3);
                    //var objectSet = session.GetObjectSet<Post>(); // holt alle Posts
                    //objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@title OR Id=@id", new { title = "Mark", id = 5 });
                    //// das gleich nur ohne anonyme Argumente
                    //objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@0 OR Id=@1", "Mark", 5);
                    //objectSet = session.GetObjectSet<Post>(post => post.Id == 4 || post.IsActive);
                    //session.GetObject<Post>(6); // holt genau einen Post mit PrimaryKey
                    //session.GetObject<Post>(post => post.Title == "Mark" && post.Id == 6); // holt alle Posts die diese Kriterien erfüllen
                    //session.GetValue<int>("select COUNT(*) from Posts"); // holt einen Wert
                    //var post2 = dbSession.GetObjectSet<string>("select Title from Posts");

                    while (Console.KeyAvailable == false)
                    {
                        var posts = dbSession.GetObjectSet<Post>();
                    }
                    Console.ReadKey();
                    //var users = dbSession.GetObjectSet<Users>();
                }

                DbSchemaAllocator.FlushReader();
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
    class Post : Entity
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
