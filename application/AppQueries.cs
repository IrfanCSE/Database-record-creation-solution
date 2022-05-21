using System.Collections.Generic;

namespace application
{
    public class AppQueries
    {
        public static List<string> Queries()
        {
            var queries = new List<string>();
            queries.Add("Insert into configuration (category, keyname, value, status) values ('test', 'key1', 'value', 1)");
            // queries.Add("Insert into configuration (category, keyname, value, status) values ('test1', 'key1', 'value-1', 1)");
            // queries.Add("Insert into configuration (category, keyname, value, status) values ('test', 'key2', 'value_2', 1)");
            // queries.Add("Insert into configuration_2 (category, keyname, value, status) values ('test', 'key1', 'value', 0)");
            // queries.Add("Insert into configuration_2 (category, keyname, value, status) values ('test1', 'key2', 'value-update', 0)");

            return queries;
        }
    }
}