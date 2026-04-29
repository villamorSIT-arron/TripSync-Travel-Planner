namespace TripSync___Travel_Planner.Data
{
    using MySql.Data.MySqlClient;
    using Microsoft.Extensions.Configuration;

    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}