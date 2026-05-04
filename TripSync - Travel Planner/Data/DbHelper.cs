namespace TripSync___Travel_Planner.Data
{
    using MySql.Data.MySqlClient;
    using Microsoft.Extensions.Configuration;

    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public bool IsTripMember(int tripId, int userId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM trip_members WHERE trip_id = @trip AND user_id = @user", conn);
            cmd.Parameters.AddWithValue("@trip", tripId);
            cmd.Parameters.AddWithValue("@user", userId);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public bool IsTripOwner(int tripId, int userId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM trip_members WHERE trip_id = @trip AND user_id = @user AND role = 'owner'", conn);
            cmd.Parameters.AddWithValue("@trip", tripId);
            cmd.Parameters.AddWithValue("@user", userId);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
