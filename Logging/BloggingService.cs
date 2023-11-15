using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logging;

public class BloggingService : IBloggingService
{
    private readonly BloggingContext _dbContext;
    private readonly ILogger<BloggingService> _logger;

    public BloggingService(
        BloggingContext dbContext,
        ILogger<BloggingService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    public void GetWithoutQueryTags()
    {
        var _ = _dbContext.Blogs.Where(x => x.BlogId == 5).ToList();
    }

    public void GetWithQueryTags()
    {
        var _ = _dbContext
            .Blogs
            .TagWith("GetBlogs")
            .ToList();
    }

    public void GetWithExtraLogs()
    {
        using (_logger.BeginScope(new Dictionary<string, object> { { "EFQueriesType", "GetBlogsValue" } }))
        {
            var _ = _dbContext
                .Blogs
                .TagWith("GetBlogs")
                .ToList();
        }
    }

    public void InsertWithoutLogScope()
    {
        var blogging = new Blog
        {
            Url = "fake_url"
        };
        _dbContext.Add(blogging);
        _dbContext.SaveChanges();
    }

    public void InsertWithLogScope()
    {
        var blogging = new Blog
        {
            Url = "fake_url"
        };
        _dbContext.Add(blogging);
        using (_logger.BeginScope(new Dictionary<string, object> { { "EFQueries", "InsertBlog" } }))
        {
            _dbContext.SaveChanges();
        }
    }

    public void ExecuteRawQueryWithoutQueryTags()
    {
        _dbContext.Blogs
            .FromSql($"SELECT * FROM Blogs")
            .ToList();
    }

    public void ExecuteRawQueryWithQueryTags()
    {
        using (_logger.BeginScope(new Dictionary<string, object> { { "EFQueries", "FromSql_AllBlog" } }))
        {
            _dbContext.Blogs
                .FromSql($"SELECT * FROM Blogs")
                .TagWith("GetBlogsRawQuery")
                .ToList();
        }
    }
}

public interface IBloggingService
{
    void GetWithoutQueryTags();
    void GetWithQueryTags();
    void GetWithExtraLogs();
    void InsertWithoutLogScope();
    void InsertWithLogScope();
    void ExecuteRawQueryWithoutQueryTags();
    void ExecuteRawQueryWithQueryTags();
}