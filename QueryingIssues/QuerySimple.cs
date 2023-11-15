namespace QueryingIssues.EF7
{
    public class QuerySimple
    {
        public void Query()
        {
            using var db = new BloggingContext();

            db.Blogs
                .Where(p => p.BlogId == 5)
                .ToList();

            // raw query: 
            //SELECT "b"."BlogId", "b"."Url"
            //  FROM "Blogs" AS "b"
            //  WHERE "b"."BlogId" = 5
        }
    }
}
