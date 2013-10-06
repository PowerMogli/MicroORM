using MicroORM.Base;
using MicroORM.Base.Mapping;
using System;

namespace Micro.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DatabaseTransaction transaction = new DatabaseTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                using (Session session = new Session(@"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True"))
                {
                    session.ExecuteCommand("select * from Posts where Title=@0 and Id=@1", "Mark", 3);

                    var objectSet = session.GetObjectSet<Post>(); // holt alle Posts
                    objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@title OR Id=@id", new { title = "Mark", id = 5 });
                    // das gleich nur ohne anonyme Argumente
                    objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@0 OR Id=@1", "Mark", 5);
                    objectSet = session.GetObjectSet<Post>(post => post.Id == 4 || post.IsActive);
                    session.GetObject<Post>(6); // holt genau einen Post mit PrimaryKey
                    session.GetObject<Post>(post => post.Title == "Mark" && post.Id == 6); // holt alle Posts die diese Kriterien erfüllen
                    session.GetValue<int>("select COUNT(*) from Posts"); // holt einen Wert
                    // Stored Procedures werde ich auch noch einbauen
                    // session.ExecuteStoredProcedure("spExecuteSomething", new { Param1 = "bla", _OutParam = 0 });

                    transaction.Commit();
                }
            }
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
