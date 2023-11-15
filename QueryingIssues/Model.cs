using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class BloggingContext : DbContext
{
    public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => builder.AddConsole());

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseLoggerFactory(MyLoggerFactory) // Enable EF Core logging
             .UseSqlServer(
                 "Server=LAPTOP-IAJ1J0A2;Database=blogpost;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");
            //.UseSqlite($"Data Source={DbPath}");
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

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
