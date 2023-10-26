using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingIssues
{
    // https://stackoverflow.com/questions/58011931/how-can-i-reuse-a-subquery-inside-a-select-expression
    public class QueryStackoverflow58011931
    {
        public async Task Query()
        {
            using var db = new BloggingContext();

            await db
               .Blogs
               .Select(blog => new
               {
                   blog.BlogId,
                   blog.Url,
                   blog.Posts.OrderBy(post => post.CreatedAt).First().Content,
                   blog.Posts.OrderBy(post => post.CreatedAt).First().Title,
               })
               .ToListAsync();

            // query:
            //SELECT "b"."BlogId", "b"."Url", (
            //  SELECT "p"."Content"
            //  FROM "Posts" AS "p"
            //  WHERE "b"."BlogId" = "p"."BlogId"
            //  ORDER BY "p"."CreatedAt"
            //  LIMIT 1) AS "Content", (
            //  SELECT "p0"."Title"
            //  FROM "Posts" AS "p0"
            //  WHERE "b"."BlogId" = "p0"."BlogId"
            //  ORDER BY "p0"."CreatedAt"
            //  LIMIT 1) AS "Title"
            //FROM "Blogs" AS "b"
        }

        public async Task Query_Workaround()
        {
            using var db = new BloggingContext();

            await db
                     .Blogs
                     .Select(blog => new
                     {
                         blog.BlogId,
                         blog.Url,
                         FirstBlog = blog.Posts.Select(x => new
                         {
                             x.Content,
                             x.Title
                         })
                         .FirstOrDefault()
                     })
                     .ToListAsync();

            // query:
          //      SELECT "b"."BlogId", "b"."Url", "t0"."Content", "t0"."Title", "t0"."c"
          //FROM "Blogs" AS "b"
          //LEFT JOIN(
          //    SELECT "t"."Content", "t"."Title", "t"."c", "t"."BlogId"
          //    FROM(
          //        SELECT "p"."Content", "p"."Title", 1 AS "c", "p"."BlogId", ROW_NUMBER() OVER(PARTITION BY "p"."BlogId" ORDER BY "p"."PostId") AS "row"
          //        FROM "Posts" AS "p"
          //    ) AS "t"
          //    WHERE "t"."row" <= 1
          //) AS "t0" ON "b"."BlogId" = "t0"."BlogId"
        }
    }
}
