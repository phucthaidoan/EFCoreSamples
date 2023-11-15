namespace QueryingIssues
{
    //https://github.com/dotnet/efcore/issues/27285
    public class QueryGitNo27285
    {
        public void Query()
        {
            using var db = new BloggingContext();

            db.Blogs
                .Select(p => new
                {
                    p.Url,
                    PostCount = p.Posts.Count
                })
                .Where(p => p.PostCount > 5)
                .ToList();

            // raw query: issue is still open
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
