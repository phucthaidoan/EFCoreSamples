using ExcludedProperties.EF8;
using Microsoft.EntityFrameworkCore;

await IgnoredFieldTestingAsync();

//await QueryRelatedFieldTestingAsync();

async Task IgnoredFieldTestingAsync()
{
    using (var dbContext = new ExcludedProperties.EF8.AppContext())
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.AddRangeAsync(new List<Person>
        {
            new Person
            {
                Age = 9,
            },
            new Person
            {
                Age = 49,
            }
        });
        await dbContext.SaveChangesAsync();
    }

    using (var dbContext = new ExcludedProperties.EF8.AppContext())
    {
        dbContext.People.Select(p => new
        {
            p.Id,
            p.Age,
            p.IsOver18
        })
        .ToList()
        .ForEach(p => Console.WriteLine($"{p.Id} - {p.Age} - {p.IsOver18}"));

        dbContext.ChangeTracker.Clear();

        // Not include Age, but still implicit query Age to compute IsOver18 => oops!
        dbContext.People.Select(p => new
        {
            p.Id,
            p.IsOver18
        })
       .ToList()
       .ForEach(p => Console.WriteLine($"No include Age property => {p.Id} - {p.IsOver18}"));
    }
}


async Task QueryRelatedFieldTestingAsync()
{
    await InitAsync();

    // notice that there is no query related table => it is a client evaluation
    using (var dbContext = new ExcludedProperties.EF8.AppContext())
    {
        var blogs = await dbContext
            .Blogs
            .Select(blog => new
            {
                blog.BlogId,
                blog.TotalNumberOfPostView
            })
            .ToListAsync();

        blogs.ForEach(blog => Console.WriteLine($"{blog.BlogId} - {blog.TotalNumberOfPostView}"));
    }

    // this is a different sample to include related table to ensure that the TotalNumberOfPostView is required to evaluate at clien side
    using (var dbContext = new ExcludedProperties.EF8.AppContext())
    {
        var blogs = await dbContext
            .Blogs
            .Include(blog => blog.Posts)
            .Select(blog => new
            {
                blog.BlogId,
                blog.TotalNumberOfPostView
            })
            .ToListAsync();

        blogs.ForEach(blog => Console.WriteLine($"{blog.BlogId} - {blog.TotalNumberOfPostView}"));
    }
}

async Task InitAsync()
{
    using var dbContext = new ExcludedProperties.EF8.AppContext();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();

    var now = DateTime.Now;
    // Generate 10 blogs
    for (int i = 0; i < 10; i++)
    {
        var blog = new Blog { Url = $"https://example.com/blog/{i + 1}" };
        await dbContext.Blogs.AddAsync(blog);

        // Generate 2 to 4 posts for each blog
        var random = new Random();
        int postCount = random.Next(2, 5);

        for (int j = 0; j < postCount; j++)
        {
            now = now.AddMinutes(1);
            var post = new Post
            {
                Title = $"Post {j + 1} for Blog {i + 1}",
                Content = $"Content for Post {j + 1}",
                CreatedAt = now,
                NumberOfView = random.Next(100, 501)
            };
            blog.Posts.Add(post);
        }
    }

    dbContext.SaveChanges();

    await PrintAsync(dbContext);
}

async Task PrintAsync(ExcludedProperties.EF8.AppContext context)
{
    var blogs = await context
        .Blogs
        .Include(blog => blog.Posts)
        .AsQueryable()
        .ToListAsync();

    foreach (var blog in blogs)
    {
        Console.WriteLine($"Blog {blog.Url} ");
        foreach (var post in blog.Posts)
        {
            Console.WriteLine($"Post ({post.PostId}): {post.Title} - {post.CreatedAt}");
        }
    }
}