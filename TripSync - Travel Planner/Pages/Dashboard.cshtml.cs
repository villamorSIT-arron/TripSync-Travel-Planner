using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly DbHelper _db;

        public DashboardModel(DbHelper db)
        {
            _db = db;
        }

        public List<TripItem> Trips { get; set; } = new();

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
            SELECT t.trip_id, t.trip_name, t.destination, t.start_date, t.end_date,
                   IFNULL(SUM(e.amount),0) AS total,
                   COUNT(DISTINCT tm_all.user_id) AS member_count
            FROM trips t
            JOIN trip_members tm ON t.trip_id = tm.trip_id
            LEFT JOIN trip_members tm_all ON t.trip_id = tm_all.trip_id
            LEFT JOIN expenses e ON t.trip_id = e.trip_id
            WHERE tm.user_id = @user
            GROUP BY t.trip_id, t.trip_name, t.destination, t.start_date, t.end_date
            ORDER BY t.start_date", conn);

            cmd.Parameters.AddWithValue("@user", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Trips.Add(new TripItem
                {
                    Id = reader.GetInt32("trip_id"),
                    Name = reader.GetString("trip_name"),
                    Destination = reader.GetString("destination"),
                    StartDate = reader.GetDateTime("start_date"),
                    EndDate = reader.GetDateTime("end_date"),
                    TotalExpense = reader.GetDecimal("total"),
                    MemberCount = reader.GetInt32("member_count")
                });
            }

            return Page();
        }

        public class TripItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Destination { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalExpense { get; set; }
            public int MemberCount { get; set; }
        }
    }
}
