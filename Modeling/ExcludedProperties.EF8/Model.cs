using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExcludedProperties.EF8
{
    public class AppContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => builder.AddConsole());

        public DbSet<Person> People { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseLoggerFactory(MyLoggerFactory) // Enable EF Core logging
            .UseSqlServer(
                "Server=LAPTOP-IAJ1J0A2;Database=backingfield_excludedProperties;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
    }

    public class Person
    {
        [NotMapped]
        public bool IsOver18 => Age > 18;

        public int Id { get; set; }

        public int Age { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        [NotMapped]
        public long TotalNumberOfPostView => Posts.Select(p => p.NumberOfView).Sum();
        public List<Post> Posts { get; } = new();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public DateTime CreatedAt { get; set; }

        public long NumberOfView { get; set; }
    }
}
