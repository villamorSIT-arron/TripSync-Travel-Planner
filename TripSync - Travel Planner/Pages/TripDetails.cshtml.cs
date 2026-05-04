using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class TripDetailsModel : PageModel
    {
        private readonly DbHelper _db;

        public TripDetailsModel(DbHelper db)
        {
            _db = db;
        }

        public int TripId { get; set; }
        public string TripName { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalExpense { get; set; }
        public int TotalActivities { get; set; }
        public List<MemberItem> Members { get; set; } = new();

        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            TripId = tripId;

            using var conn = _db.GetConnection();
            conn.Open();

            var tripCmd = new MySqlCommand(
                "SELECT trip_name, destination, start_date, end_date FROM trips WHERE trip_id=@id", conn);
            tripCmd.Parameters.AddWithValue("@id", tripId);

            using (var reader = tripCmd.ExecuteReader())
            {
                if (!reader.Read())
                    return RedirectToPage("/Dashboard");

                TripName = reader.GetString("trip_name");
                Destination = reader.GetString("destination");
                StartDate = reader.GetDateTime("start_date");
                EndDate = reader.GetDateTime("end_date");
            }

            var cmd1 = new MySqlCommand(
                "SELECT IFNULL(SUM(amount),0) FROM expenses WHERE trip_id=@id", conn);
            cmd1.Parameters.AddWithValue("@id", tripId);
            TotalExpense = Convert.ToDecimal(cmd1.ExecuteScalar());

            var cmd2 = new MySqlCommand(
                "SELECT COUNT(*) FROM itinerary WHERE trip_id=@id", conn);
            cmd2.Parameters.AddWithValue("@id", tripId);
            TotalActivities = Convert.ToInt32(cmd2.ExecuteScalar());

            var membersCmd = new MySqlCommand(@"
                SELECT u.name, u.email, tm.role
                FROM trip_members tm
                JOIN users u ON tm.user_id = u.user_id
                WHERE tm.trip_id = @id
                ORDER BY tm.role DESC, u.name", conn);
            membersCmd.Parameters.AddWithValue("@id", tripId);

            using var membersReader = membersCmd.ExecuteReader();
            while (membersReader.Read())
            {
                Members.Add(new MemberItem
                {
                    Name = membersReader.GetString("name"),
                    Email = membersReader.GetString("email"),
                    Role = membersReader.IsDBNull(membersReader.GetOrdinal("role")) ? "member" : membersReader.GetString("role")
                });
            }

            return Page();
        }

        public class MemberItem
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
