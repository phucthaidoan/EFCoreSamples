using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackingFields.ComputedField.EF8
{
    public class AppContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => builder.AddConsole());

        public DbSet<Person> People { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Person>()
                .Property(p => p.IsOver18)
                .HasField("_isOver18");

            modelBuilder
                .Entity<Order>()
                .Property("_orderDate")
                .HasColumnName("OrderDate");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseLoggerFactory(MyLoggerFactory) // Enable EF Core logging
            .UseSqlServer(
                "Server=LAPTOP-IAJ1J0A2;Database=backingfield_computed;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
    }

    public class Person
    {
        private bool _isOver18;
        private int _age;
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool IsOver18 { get; private set; }

        public int Id { get; set; }

        public int Age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
                if (_age < 6)
                {
                    throw new Exception("Not allow person whose age is less than 6");
                }

                if (_age > 18)
                {
                    _isOver18 = true;
                }
                else
                {
                    _isOver18 = false;
                }
            }
        }
    }

    public class Order
    {
        private DateTime _orderDate;

        public int Id { get; set; }
        public void SetOrderDate(DateTime dateTime)
        {
            if (dateTime < DateTime.Today)
            {
                throw new Exception("Order date cannot be in the past");
            }

            _orderDate = dateTime;
        }

        public DateTime GetOrderDate() => _orderDate;
    }
}
