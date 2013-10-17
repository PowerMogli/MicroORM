using System;
using System.Data;
using MicroORM.Attributes;
using MicroORM.Base;
using MicroORM.Query;

namespace Micro.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //ConnectionStringRegistrar.Register("Micro.Program.*", @"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True");
                //DbEngineRegistrar.Register("Micro.Program.*", DbEngine.SqlServer);

                //Post post = new Post();
                //post.Id = 6;
                //post.Title = "bla";
                //post.Load();
                //ConnectionStringRegistrar.Flush();
                //DbEngineRegistrar.Flush();

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
                    //var post2 = dbSession.GetObjectSet<string>("select Title from Posts");

                    Post post = new Post();
                    post.Title = "Die Spiele von Yoda";
                    post.Type = PostType.Post;
                    post.AuthorId = 382;
                    post.IsActive = true;
                    post.TopicId = 32;
                    post.CreatedOn = DateTime.Now.AddDays(-232);
                    dbSession.Insert(post);
                    //var post = dbSession.GetObjectSet<Post>();
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
        [Column]
        [PrimaryKey]
        public int Id { get; set; }
        [Column]
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
    class Post
    {
        public Post()
        {
            CreatedOn = DateTime.Now;

        }
        [Column(AutoNumber = true)]
        [PrimaryKey]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public PostType Type { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }
    }

    public enum PostType
    {
        Post,
        Page
    }
}
