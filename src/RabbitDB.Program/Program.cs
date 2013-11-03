using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
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
                //List<string> list = new List<string> { "abc", "def", "ghi" };
                //Expression<Func<Post, bool>> expression = post => !post.IsActive && post.CreatedOn == DateTime.Now;
                //TryExpression<Post>(post => !post.IsActive);
                Registrar<DbEngine>.Register("RabbitDB.Program.*", DbEngine.SqlServer);
                Registrar<string>.Register("RabbitDB.Program.*", @"Data Source=ASLUPIANEKW764\SQLEXPRESS;Initial Catalog=AdventureWorks2012;Integrated Security=True");
                DbSession.Configuration.AutoDetectChangesEnabled = false;

                using (DbSession dbSession = new DbSession(typeof(Program)))
                {
                    //    //session.ExecuteCommand("select * from Posts where Title=@0 and Id=@1", "Mark", 3);
                    //    //var objectSet = session.GetObjectSet<Post>(); // holt alle Posts
                    //    //objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@title OR Id=@id", new { title = "Mark", id = 5 });
                    //    //// das gleich nur ohne anonyme Argumente
                    //    //objectSet = session.GetObjectSet<Post>("select * from Posts where Title=@0 OR Id=@1", "Mark", 5);
                    //    //objectSet = session.GetObjectSet<Post>(post => post.Id == 4 || post.IsActive);
                    //    //session.GetObject<Post>(6); // holt genau einen Post mit PrimaryKey
                    //    //session.GetObject<Post>(post => post.Title == "Mark" && post.Id == 6); // holt alle Posts die diese Kriterien erfüllen
                    //    //session.GetValue<int>("select COUNT(*) from Posts"); // holt einen Wert
                    //    //var post2 = dbSession.GetObjectSet<string>("select Title from Posts");

                    var sql = @"SELECT * FROM Posts WHERE Type=@Type AND IsActive=@Active;
                                SELECT * FROM Users WHERE Id=@Id;";

                    var multiSet = dbSession.ExecuteMultiple(sql, new { Id = 1, Type = PostType.Post, Active = true });
                    var posts = multiSet.Read<Post>();
                    var users = multiSet.Read<Users>();
                }
                DbSchemaAllocator.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        static void post_EntityUpated(object sender, EventArgs e)
        {
            Post post = sender as Post;
            Console.WriteLine("Post with title: {0} was updated", post.Title);
        }

        static IEnumerable<Post> CustomMap(IDataReader dataReader)
        {
            List<Post> list = new List<Post>();
            while (dataReader.Read())
            {
                Post entity = new Post();
                entity.Id = (int)dataReader["Id"];
                entity.Title = (string)dataReader["Title"];
                entity.AuthorId = (int)dataReader["AuthorId"];
                entity.CreatedOn = (DateTime)dataReader["CreatedOn"];
                entity.IsActive = (bool)dataReader["IsActive"];
                entity.TopicId = new Nullable<int>(dataReader["TopicId"] != DBNull.Value ? (int)dataReader["TopicId"] : 0);
                entity.Type = dataReader["Type"] != DBNull.Value ? (PostType)Convert.ToByte(dataReader["Type"]) : default(PostType);
                list.Add(entity);
            }

            return list;
        }

        static void TryExpression<T>(Expression<Func<T, bool>> criteria)
        {
            criteria.Body.BelongsToParameter();
        }
    }
    static class ExpressionExtensions
    {
        public static bool BelongsToParameter(this Expression node, Type type = null)
        {
            Expression parent = null;

            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    parent = ((MemberExpression)node).Expression;
                    break;
                case ExpressionType.Call:
                    var m = (MethodCallExpression)node;
                    parent = m.Object;
                    if (m.HasParameterArgument(type))
                    {
                        return true;
                    }
                    break;
                case ExpressionType.Not:
                case ExpressionType.Convert:
                    var u = node as UnaryExpression;
                    parent = u.Operand;
                    break;
            }

            if (parent == null) return false;

            if (parent.NodeType != ExpressionType.Parameter)
            {
                return parent.BelongsToParameter(type);
            }

            if (type != null)
            {
                if (parent.Type != type) return false;
            }
            return true;
        }

        public static bool HasParameterArgument(this MethodCallExpression node, Type type = null)
        {
            foreach (var arg in node.Arguments)
            {
                if (arg.IsParameter(type) || arg.BelongsToParameter(type)) return true;
            }
            return false;
        }

        public static bool IsParameter(this Expression ex, Type type = null)
        {
            if (ex.NodeType != ExpressionType.Parameter) return false;
            if (type == null) return true;
            var par = ex as ParameterExpression;
            return (par.Type == type);
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
