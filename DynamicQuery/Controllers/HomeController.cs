using DynamicQuery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
//using System.Web.Mvc;

namespace DynamicQuery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;                
        RMS_DBContext db = new RMS_DBContext();
        
        private string[] RESTRICTED_DML_STATEMENTS = { "UPDATE", "DELETE", "INSERT" , "MERGE", "DROP", "DESCRIBE" };
        private SQLParser SQLParser = new SQLParser();


        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
        }
        /**
        * TODO:
        * BackEnd
        * 1. Create Database Model
        * 2. Migrate ConnectionString to Webconfig
        * 
        * FrontEnd
        * 1. StoredQuery CRUD
        * 2. Dynamic Data-table
        * 3. Export CSV Function, Copy Fnction
        * 
        * 
        * Recurring data 
        */
        public IActionResult Index()
        {
            List<StoredSqlquery> remQueries = db.StoredSqlqueries.ToList() ;
            return View(remQueries);
        }

        public PartialViewResult _Parameter(int id) {

            StoredSqlquery data = db.StoredSqlqueries.Find(id) ;

            return PartialView(data);
        }
        public JsonResult Query(int query_id, Dictionary<string, string> parameter) {

            List<dynamic> data = null;
            StoredSqlquery _remQueries = db.StoredSqlqueries.Find(query_id);
            try
            {
                string query = _remQueries.Query.Replace("\n", " ").Replace("\r", " ");
                foreach (string STATEMENT in RESTRICTED_DML_STATEMENTS)
                {
                    if (_remQueries.Query.ToUpper().Contains(STATEMENT))
                        throw new Exception("Query statement is not allowed");
                }

                //string connection = ConfigurationManager.ConnectionStrings[_remQueries.Connection].ConnectionString;
                string connection = "";
                using (IDbConnection db = new SqlConnection(connection))
                {
                    var dictionary = new DynamicParameters();

                    foreach (var item in parameter)
                    {
                        if (item.Key == "query_id" || item.Value == "") continue;
                        if (item.Value.Contains(","))
                        {
                            List<string> ss = new List<string>();
                            string[] ss2 = item.Value.Replace("\n", "").Replace("\r", "").Split(',');
                            // remove new line
                            foreach (string s2 in ss2)
                            {
                                ss.Add(s2);
                            }
                            dictionary.Add("@" + item.Key, ss);
                        }
                        else
                        {
                            dictionary.Add("@" + item.Key, item.Value);
                        }
                    }
                    data = db.Query<dynamic>(query, dictionary).ToList();
                    if (data.Count == 0)
                    {
                        List<dynamic> obj = new List<dynamic>();
                        List<dynamic> obj2 = new List<dynamic>();
                        obj.Add(new
                        {
                            Key = "Empty",
                            Value = "No Result"
                        }); ;
                        obj2.Add(obj);
                        data = obj;
                    }
                }

            }
            catch (Exception e)
            {
                List<dynamic> obj = new List<dynamic>();
                List<dynamic> obj2 = new List<dynamic>();
                obj.Add(new
                {
                    Key = "Error",
                    Value = e.Message.ToString()
                }); ;
                obj2.Add(obj);
                data = obj;
            }
            var result = Json(data);
            return result;
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
    }

    

}
