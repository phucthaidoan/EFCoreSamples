using Microsoft.EntityFrameworkCore;

namespace QueryingIssues
{
    public class QueryProjectionAsNoTracking
    {
        /// <summary>
        /// If using projection query, then EF Core doesn't track any entities result
        /// </summary>
        public void Query()
        {
            Init();

            using var db = new BloggingContext();

            var listWithoutAsNoTracking = db
                .Blogs
                .ToList();
            var noOfTrackedEntities = db.ChangeTracker.Entries().Count();
            Console.WriteLine($"After query without AsNoTracking() - {noOfTrackedEntities}");

            var findBlogId1 = db.Blogs.Find(1);
            Console.WriteLine($"After that using .Find() to query => check query: no sql generated!!!");
            db.ChangeTracker.Clear();

            var listWithAsNoTracking = db
                .Blogs
                .AsNoTracking()
                .ToList();

            noOfTrackedEntities = db.ChangeTracker.Entries().Count();
            Console.WriteLine($"After query with AsNoTracking() - {noOfTrackedEntities}");
            db.ChangeTracker.Clear();

            var listNoAsNoTrackingProjection = db
                    .Blogs
                    .Select(p => new
                    {
                        p.Url,
                        p.BlogId
                    })
                    .ToList();
            noOfTrackedEntities = db.ChangeTracker.Entries().Count();
            Console.WriteLine($"After query using projection without AsNoTracking() - {noOfTrackedEntities}");
            db.ChangeTracker.Clear();

            var listNoAsNoTrackingProjectionWithInclude = db
                    .Blogs
                    .Include(blog => blog.Posts)
                    .Select(p => new
                    {
                        p.BlogId
                    })
                    .ToList();
            noOfTrackedEntities = db.ChangeTracker.Entries().Count();
            Console.WriteLine($"After query blogs that include posts using projection without AsNoTracking() - {noOfTrackedEntities}");

            var listNoAsNoTrackingProjectionWithEntity = db
                    .Blogs
                    .Select(blog => new
                    {
                        Blog = blog,
                        HasPost = blog.Posts
                    })
                    .ToList();
            noOfTrackedEntities = db.ChangeTracker.Entries().Count();
            Console.WriteLine($"After query blogs that using project query of blog entity without AsNoTracking() - {noOfTrackedEntities}");
        }

        private void Init()
        {
            using var context = new BloggingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var now = DateTime.Now;
            // Generate 10 blogs
            for (int i = 0; i < 10; i++)
            {
                var blog = new Blog { Url = $"https://example.com/blog/{i + 1}" };
                context.Blogs.Add(blog);

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

            context.SaveChanges();

            Print(context);
        }

        private void Print(BloggingContext context)
        {
            foreach (var blog in context.Blogs)
            {
                Console.WriteLine($"Blog {blog.Url} ");
                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($"Post ({post.PostId}): {post.Title} - {post.CreatedAt}");
                }
            }
        }
    }
}
