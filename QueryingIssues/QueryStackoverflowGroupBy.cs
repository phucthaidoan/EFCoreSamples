using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingIssues.EF7
{
    public class QueryStackoverflowGroupBy
    {
        // https://stackoverflow.com/questions/69355421/retrieving-the-last-record-in-each-group-with-ef-core
        // https://stackoverflow.com/questions/59456026/how-to-select-top-n-rows-for-each-group-in-a-entity-framework-groupby-with-ef-3/59468439#59468439
        public void QueryNo59456026()
        {
            Init();

            using var context = new BloggingContext();

            // This is just a demo to see how groupBy is improved in EF Core > 3.3
            var data = context
                .Posts
                .GroupBy(post => post.BlogId)
                .Select(group => new
                {
                    BlogId = group.Key,
                    Post = group.OrderByDescending(post => post.CreatedAt).FirstOrDefault()
                })
                .ToList();

            foreach (var item in data)
            {
                Console.WriteLine($"Blog {item.BlogId} - Post ({item.Post?.PostId}): {item.Post?.Title}");
            }
        }

        public void Query()
        {
            Init();

            using var context = new BloggingContext();

            var result = context
                .Blogs
                .AsQueryable()
                .Select(blog => new
                {
                    BlogUrl = blog.Url,
                    Post = blog.Posts.OrderByDescending(post => post.CreatedAt).Select(post => new
                    {
                        post.PostId,
                        post.Title
                    })
                    .FirstOrDefault()
                })
                .ToList();

            foreach ( var blog in result )
            {
                Console.WriteLine($"Blog {blog.BlogUrl} - Post ({blog.Post?.PostId}): {blog.Post?.Title}");
            }

            // It can be a better query to get the oldest create post of each blog
            /*var data2 = context
                .Blogs
                .Select(blog => new
                {
                    blog.BlogId,
                    Post = blog.Posts.OrderByDescending(blog => blog.CreatedAt).FirstOrDefault()
                })
                .ToList();

            Console.WriteLine("-------------------------------");
            foreach (var item in data)
            {
                Console.WriteLine($"Blog {item.BlogId} - Post ({item.Post?.PostId}): {item.Post?.Title}");
            }*/
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
            foreach(var blog in context.Blogs)
            {
                Console.WriteLine($"Blog {blog.Url} ");
                foreach(var post in blog.Posts)
                {
                    Console.WriteLine($"Post ({post.PostId}): {post.Title} - {post.CreatedAt}");
                }
            }
        }
    }
}
