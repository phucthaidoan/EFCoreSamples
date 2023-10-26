using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingIssues
{
    //https://github.com/dotnet/efcore/issues/27285
    public class QueryGitNo27285
    {
        public async Task Query()
        {
            using var db = new BloggingContext();

            await db
                    .Blogs
                    .Select(p => new
                    {
                        p.Url,
                        PostCount = p.Posts.Count
                    })
                    .Where(p => p.PostCount > 5)
                    .ToListAsync();

            // raw query: 
          //  SELECT "b"."Url", (
          //SELECT COUNT(*)
          //FROM "Posts" AS "p0"
          //WHERE "b"."BlogId" = "p0"."BlogId") AS "PostCount"
          //FROM "Blogs" AS "b"
          //WHERE(
          //    SELECT COUNT(*)
          //    FROM "Posts" AS "p"
          //    WHERE "b"."BlogId" = "p"."BlogId") > 5
        }
    }
}
