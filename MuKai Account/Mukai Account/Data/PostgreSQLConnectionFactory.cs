using System.Data;
using Chloe.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mukai_Account.Data
{
    public class PostgreSQLConnectionFactory : IDbConnectionFactory
    {
        private readonly string connection;

        public PostgreSQLConnectionFactory(string connection)
        {
            this.connection = connection;
        }
        public IDbConnection CreateConnection() => new NpgsqlConnection(this.connection);

    }
}
