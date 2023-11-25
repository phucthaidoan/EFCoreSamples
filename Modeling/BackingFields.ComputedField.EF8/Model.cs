using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackingFields.EF8.Simple
{
    public class PersonContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => builder.AddConsole());
        public string DbPath { get; }

        public DbSet<Person> People { get; set; }

        public PersonContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.IsOver18)
                .HasField("_isOver18");
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
}
