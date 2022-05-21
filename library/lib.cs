using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace library
{
    public class lib
    {
        internal static int ExecuteWrite(string Query)
        {
            using (var connection = new SqliteConnection(Configuration.ConnectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = Query;

                connection.Open();
                var result = command.ExecuteNonQuery();
                connection.Close();

                return result;
            }
        }

        internal static object ExecuteRead(string Query)
        {
            using (var connection = new SqliteConnection(Configuration.ConnectionString))
            {
                object result = null;
                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.CommandType = CommandType.Text;

                connection.Open();

                var reader = command.ExecuteReader();
                // var col = reader.FieldCount;
                // reader is treated like a array so call 
                while (reader.Read())
                {
                    // for simplicity
                    result = reader[0];
                    
                    // for (int i = 0; i < col; i++)
                    // {
                    // }
                }

                // Clean up no longer needed connection
                connection.Close();

                return result;
            }
        }

        internal static List<string> ExecuteReadPks(string table)
        {
            var Query = $"SELECT l.name pk FROM pragma_table_info('{table}') as l WHERE l.pk != 0;";

            using (var connection = new SqliteConnection(Configuration.ConnectionString))
            {
                List<string> pks = new();
                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.CommandType = CommandType.Text;

                connection.Open();

                var reader = command.ExecuteReader();

                // reader is treated like a array so call 
                while (reader.Read())
                {
                    pks.Add(reader["pk"].ToString());
                }

                // Clean up no longer needed connection
                connection.Close();

                return pks;
            }
        }
    }
}