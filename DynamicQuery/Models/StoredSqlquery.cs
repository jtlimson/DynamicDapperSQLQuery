using System;
using System.Collections.Generic;

#nullable disable

namespace DynamicQuery.Models
{
    public partial class StoredSqlquery
    {

        public int Id { get; set; }
        /// <summary>
        /// RAW SQL Query
        /// Sample 
        /// SELECT * FROM StoredSqlquery WHERE Id=@ID
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Readable Description of the Query
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Parameter of the statement for the Query. e.g. ) WHERE Id = @id
        /// So the parameter is the id without the "@". delimited by comma (,)
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// ConnectionString of the query
        /// Server=MyServer;Database=MyDatabase;Persist Security Info=True;User ID=SA;Password=PASSWORD;MultipleActiveResultSets=True;
        /// </summary>
        public string Connection { get; set; } 
        
    }
}
