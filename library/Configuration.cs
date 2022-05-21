using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace library
{
    internal class Configuration
    {
        private Configuration()
        { }

        public static string ConnectionString { get; set; }
        public static List<string> Queries { get; set; }
        private static List<string> QueriesToExecute { get; set; } = new();
        private static Dictionary<string, List<string>> QueriesWithTable { get; set; } = new();

        public static void MigrateQueries()
        {
            if (Queries.Count <= 0)
                return;

            QuerieGroupByTable();

            // Convert insert queries into update queries
            ParseQueries();

            QueriesToExecute.ForEach(x => lib.ExecuteWrite(x));
        }

        private static void ParseQueries()
        {
            foreach (var item in QueriesWithTable)
            {
                var pks = lib.ExecuteReadPks(item.Key);

                // generate into update queries
                ProcessQuery(item, pks);
            }
        }

        private static void ProcessQuery(KeyValuePair<string, List<string>> item, List<string> pks)
        {
            foreach (var i in item.Value)
            {
                var querie = new StringBuilder();
                var querie_trim = i.Split(new[] { item.Key }, StringSplitOptions.RemoveEmptyEntries);
                var parse = querie_trim[1];
                var param = ParseQuerieParam(ref parse);
                var values = ParseQuerieParam(ref parse);

                var SplitedParam = param.Split(',');
                var SplitedValue = values.Split(',');

                // Trimed params & values
                TrimValueFromDb(SplitedParam, SplitedValue);

                // Check if configuration already EXISTS
                object check = CheckIfDataExist(item, pks, querie, SplitedParam, SplitedValue);

                if ((long)check == 0)
                {
                    // insert into querie
                    QueriesToExecute.Add(i);
                    continue;
                }

                // reset the querie
                querie.Clear();

                // If not then generate update query
                ConvertQueuryIntoUpdate(item, pks, querie, SplitedParam, SplitedValue);

                QueriesToExecute.Add(querie.ToString());
            }
        }

        private static void ConvertQueuryIntoUpdate(KeyValuePair<string, List<string>> item, List<string> pks, StringBuilder querie, string[] SplitedParam, string[] SplitedValue)
        {
            querie.Append($"UPDATE {item.Key} SET ");

            for (var it = 0; it < SplitedParam.Length; it++)
            {
                if (it == 0)
                    querie.Append($"{SplitedParam[it]}={SplitedValue[it]}");
                else
                    querie.Append($",{SplitedParam[it]}={SplitedValue[it]}");

            }

            AddWhereConditions(pks, querie, SplitedParam, SplitedValue);
        }

        private static object CheckIfDataExist(KeyValuePair<string, List<string>> item, List<string> pks, StringBuilder querie, string[] SplitedParam, string[] SplitedValue)
        {
            querie.Append($"SELECT EXISTS (SELECT * FROM {item.Key}");
            AddWhereConditions(pks, querie, SplitedParam, SplitedValue);
            querie.Append($" )");
            var check = lib.ExecuteRead(querie.ToString());
            return check;
        }

        private static void TrimValueFromDb(string[] SplitedParam, string[] SplitedValue)
        {
            for (var it = 0; it < SplitedParam.Length; it++)
            {
                SplitedParam[it] = SplitedParam[it].Trim();
                SplitedValue[it] = SplitedValue[it].Trim();
            }
        }

        private static void QuerieGroupByTable()
        {
            // Check if queries are already executed
            // grouped by table name for furhter processing
            Queries.ForEach(x =>
            {
                // Setup Queries with their table
                if (x.ToUpper().Contains("INSERT INTO"))
                {
                    string[] list = ParseInsertIntoRandom(x);
                    var parse = list[0].Trim().Split(' ');
                    var table = parse[0].Trim();

                    if (QueriesWithTable.ContainsKey(table))
                    {
                        QueriesWithTable[table].Add(x);
                    }
                    else
                    {
                        QueriesWithTable.Add(table, new List<string> { x });
                    }
                }
                else
                {
                    // Assuming as normal queries
                    QueriesToExecute.Add(x);
                }
            });
        }

        private static void AddWhereConditions(List<string> pks, StringBuilder querie, string[] SplitedParam, string[] SplitedValue)
        {
            for (var it = 0; it < pks.Count; it++)
            {
                pks[it] = pks[it].Trim();

                var index = Array.IndexOf(SplitedParam, pks[it]);
                if (it == 0)
                    querie.Append($" WHERE {pks[it]}={SplitedValue[index]} ");
                else
                    querie.Append($"AND {pks[it]}={SplitedValue[index]} ");
            }
        }

        // To get table information
        private static string[] ParseInsertIntoRandom(string x)
        {
            var list = x.Split(new[] { "INSERT INTO" }, StringSplitOptions.RemoveEmptyEntries);
            list = x.Split(new[] { "insert into" }, StringSplitOptions.RemoveEmptyEntries);
            list = x.Split(new[] { "Insert Into" }, StringSplitOptions.RemoveEmptyEntries);
            list = x.Split(new[] { "Insert into" }, StringSplitOptions.RemoveEmptyEntries);
            return list;
        }

        // parsing querie param and values
        private static string ParseQuerieParam(ref string parse)
        {
            string FinalString;
            int Pos1 = parse.IndexOf("(") + 1;
            int Pos2 = parse.IndexOf(")");
            FinalString = parse.Substring(Pos1, Pos2 - Pos1);

            parse = parse.Remove(Pos1, Pos2 - Pos1);
            parse = parse.Replace("()", "");

            return FinalString;
        }
    }
}