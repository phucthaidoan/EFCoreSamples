﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingIssues.EF3._1
{
    public class QueryStackoverflowGroupBy
    {
        // https://stackoverflow.com/questions/69355421/retrieving-the-last-record-in-each-group-with-ef-core
        // https://stackoverflow.com/questions/59456026/how-to-select-top-n-rows-for-each-group-in-a-entity-framework-groupby-with-ef-3/59468439#59468439
        public void QueryNo59456026()
        {
            Init();

            using var context = new BloggingContext();

            var data = context
                .Posts
                .AsNoTracking()
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

        public void QueryNo59456026_Workaround()
        {
            Init();

            using var context = new BloggingContext();

            var unique = context
                .Posts
                .AsNoTracking()
                .Select(post => new { post.BlogId })
                .Distinct()
                .ToList();

            var query =
                from u in unique
                from l in context
                    .Posts
                    .AsNoTracking()
                        .Where(post => post.BlogId == u.BlogId)
                        .OrderByDescending(post => post.CreatedAt)
                        .Take(1)
                select l;


            var data = query.ToList();

            foreach (var post in data)
            {
                Console.WriteLine($"Blog {post.BlogId} - Post ({post.PostId}): {post.Title}");
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

            foreach (var blog in result)
            {
                Console.WriteLine($"Blog {blog.BlogUrl} - Post ({blog.Post?.PostId}): {blog.Post?.Title}");
            }
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
