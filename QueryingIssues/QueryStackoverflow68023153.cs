using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryingIssues
{
    // https://stackoverflow.com/questions/68023153/remove-duplicated-sum-subquery-in-entity-framework-core-query
    public class QueryStackoverflow68023153
    {
        public async Task Query()
        {
            using var db = new BloggingContext();

            await db
                .Blogs
                .Include(blog => blog.Posts)
                .Select(blog => new
                {
                    blog.BlogId,
                    blog.Url,
                    TotalView = blog.Posts.Sum(post => post.NumberOfView)
                })
                .Where(x => x.TotalView > 1 && x.TotalView > 5)
                .Select(x => x)
                .ToListAsync();

            // QUERY:
            //SELECT "b"."BlogId", "b"."Url", (
            // SELECT COALESCE(SUM("p1"."NumberOfView"), 0)
            //  FROM "Posts" AS "p1"
            //  WHERE "b"."BlogId" = "p1"."BlogId") AS "TotalView"
            //  FROM "Blogs" AS "b"
            //  WHERE(
            //      SELECT COALESCE(SUM("p"."NumberOfView"), 0)
            //      FROM "Posts" AS "p"
            //      WHERE "b"."BlogId" = "p"."BlogId") > 1 AND(
            //      SELECT COALESCE(SUM("p0"."NumberOfView"), 0)
            //      FROM "Posts" AS "p0"
            //      WHERE "b"."BlogId" = "p0"."BlogId") > 5
        }

        public async Task Query_Workaround()
        {
            using var db = new BloggingContext();

            await db.Posts
                .GroupBy(post => post.BlogId)
                .Select(group => new { BlogId = group.Key, TotalView = group.Sum(x => x.NumberOfView) })
                .Join(
                    db.Blogs,
                    grouped => grouped.BlogId,
                    blog => blog.BlogId,
                    (grouped, blog) => new { blog.BlogId, blog.Url, grouped.TotalView })
                .Where(x => x.TotalView > 1 && x.TotalView > 5)
                .ToListAsync();

            // notice:
            // let be aware a certain case about the performance as JOIN over the Principle in the table twice

            // query:
            //SELECT "b"."BlogId", "b"."Url", "t"."TotalView"
            //  FROM(
            //      SELECT "p"."BlogId", COALESCE(SUM("p"."NumberOfView"), 0) AS "TotalView"
            //      FROM "Posts" AS "p"
            //      GROUP BY "p"."BlogId"
            //  ) AS "t"
            //  INNER JOIN "Blogs" AS "b" ON "t"."BlogId" = "b"."BlogId"
            //  WHERE "t"."TotalView" > 1 AND "t"."TotalView" > 5
        }
    }
}
