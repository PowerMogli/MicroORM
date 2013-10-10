using System;
using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Reflection;
using System.Reflection;
using MicroORM.Storage;

namespace Micro.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (DbSession dbSession = new DbSession(@"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True"))
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

                    var post = dbSession.GetObjectSet<string>("select Title from Posts");
                }
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

    [Table("Users")]
    class Users
    {
        [Field(Identifier = true)]
        public int Id { get; set; }
        [Field]
        public string Name { get; set; }
    }

    public class ImportPrepareProcedureObject : SqlProcedureObject
    {
        public ImportPrepareProcedureObject()
            : base("spExecuteSomething") { }

        public string TicketID
        {
            get { return base.GetParameterValue<string>("@pTicketID"); }
            set { base.AddParameter("@pTicketID", value, DbType.AnsiString, 255); }
        }
    }

    [Table("Posts", PrimaryKey = "Id")]
    class Post
    {
        public Post()
        {
            CreatedOn = DateTime.Now;

        }
        [Field("Id", Identifier = true, IsNullable = false)]
        public int Id { get; set; }
        [Field]
        public int AuthorId { get; set; }
        [Field]
        public string Title { get; set; }
        [Field]
        public DateTime CreatedOn { get; set; }
        //[InsertAsString]
        [Field]
        public PostType Type { get; set; }
        [Field]
        public int? TopicId { get; set; }
        [Field]
        public bool IsActive { get; set; }
    }

    public enum PostType
    {
        Post,
        Page
    }
}
