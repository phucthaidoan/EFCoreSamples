using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QueryingIssues;
using QueryingIssues.EF7;

//var query1 = new QueryGitNo27285();
//await query1.Query();

//var query2 = new QueryStackoverflow58011931();
//await query2.Query();
//await query2.Query_Workaround();

//var query3 = new QueryStackoverflow68023153();
//await query3.Query(); 
//await query3.Query_Workaround(); 

//var query4 = new QueryStackoverflowGroupBy();
//query4.QueryNo59456026();

var query5 = new QueryProjectionAsNoTracking();
query5.Query();
