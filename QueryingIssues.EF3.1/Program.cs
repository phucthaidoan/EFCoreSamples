using System;

namespace QueryingIssues.EF3._1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var query5 = new QueryStackoverflowGroupBy();
            query5.QueryNo59456026();
            query5.QueryNo59456026_Workaround();
        }
    }
}
