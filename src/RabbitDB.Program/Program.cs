using System;
using System.Data;
using RabbitDB.Attributes;
using RabbitDB.Base;
using RabbitDB.Entity;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Storage;
using System.Diagnostics;

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
                //DbSession.Configuration.AutoDetectChangesEnabled = false;

                using (DbSession dbSession = new DbSession(typeof(Program)))
                {
                    var post = new Post();
                    post.Id = 23;
                    post.CreatedOn = DateTime.Now.AddDays(-1);
                    post.Title = "I wanna fuck you";
                    dbSession.Update(post);

                    var user = new Users();
                    user.Name = "Pep Guardiola";
                    dbSession.Update(user);
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
            set { base.AddParameter("@pTicketID", value, DbType.AnsiString, parameterDirection: ParameterDirection.Output); }
        }

        public Guid ProcessGuid
        {
            get { return base.GetParameterValue<Guid>("@pProcessGuid"); }
            set { base.AddParameter("@pProcessGuid", value, DbType.Guid); }
        }
    }

    [Table("Posts")]
    class Post
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

    class Product : Entity.Entity
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public bool MakeFlag { get; set; }
        public bool FinishedGoodsFlag { get; set; }
        public string Color { get; set; }
        public short SafetyStockLevel { get; set; }
        public short ReorderPoint { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public string SizeUnitMeasureCode { get; set; }
        public string WeightUnitMeasure { get; set; }
        public decimal Weight { get; set; }
        public int DaysToManufacture { get; set; }
        public string ProductLine { get; set; }
        public string Class { get; set; }
        public string Style { get; set; }
        public int ProductSubcategory { get; set; }
        public int ProductModelID { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime SellEndDate { get; set; }
        public DateTime DiscountinuedDate { get; set; }
    }

    class SalesOrderDetail : Entity.Entity
    {
        public int SalesOrderID { get; set; }
        public int SalesOrderDetailID { get; set; }
        public string CarrierTrackingNumber { get; set; }
        public short OrderQty { get; set; }
        public int ProductID { get; set; }
        public int SpecialOfferID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceDiscount { get; set; }
        public decimal LineTotal { get; set; }
    }
}