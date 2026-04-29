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

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return;

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(@"
            SELECT t.trip_id, t.trip_name, t.destination,
                   IFNULL(SUM(e.amount),0) AS total
            FROM trips t
            JOIN trip_members tm ON t.trip_id = tm.trip_id
            LEFT JOIN expenses e ON t.trip_id = e.trip_id
            WHERE tm.user_id = @user
            GROUP BY t.trip_id", conn);

            cmd.Parameters.AddWithValue("@user", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Trips.Add(new TripItem
                {
                    Id = reader.GetInt32("trip_id"),
                    Name = reader.GetString("trip_name"),
                    Destination = reader.GetString("destination"),
                    TotalExpense = reader.GetDecimal("total")
                });
            }
        }

        public class TripItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Destination { get; set; }
            public decimal TotalExpense { get; set; }
        }
    }
}
