using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace library
{
    public static class SetupLibrary
    {
        public static void AddLibrary(this IServiceCollection services, string ConnectionString, List<string> Queries)
        {
            Configuration.ConnectionString = ConnectionString;
            Configuration.Queries = Queries;

            Configuration.MigrateQueries();
        }
    }
}