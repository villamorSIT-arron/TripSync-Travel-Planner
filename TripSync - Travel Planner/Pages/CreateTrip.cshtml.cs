using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class CreateTripModel : PageModel
    {
        private readonly DbHelper _db;

        public CreateTripModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty] public string TripName { get; set; }
        [BindProperty] public string Destination { get; set; }
        [BindProperty] public DateTime StartDate { get; set; }
        [BindProperty] public DateTime EndDate { get; set; }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                @"INSERT INTO trips (trip_name, destination, start_date, end_date, created_by)
              VALUES (@name, @dest, @start, @end, @user)", conn);

            cmd.Parameters.AddWithValue("@name", TripName);
            cmd.Parameters.AddWithValue("@dest", Destination);
            cmd.Parameters.AddWithValue("@start", StartDate);
            cmd.Parameters.AddWithValue("@end", EndDate);
            cmd.Parameters.AddWithValue("@user", userId);

            cmd.ExecuteNonQuery();

            long tripId = cmd.LastInsertedId;

            var memberCmd = new MySqlCommand(
                "INSERT INTO trip_members (trip_id, user_id, role) VALUES (@trip, @user, 'owner')", conn);

            memberCmd.Parameters.AddWithValue("@trip", tripId);
            memberCmd.Parameters.AddWithValue("@user", userId);

            memberCmd.ExecuteNonQuery();

            return RedirectToPage("/Dashboard");
        }
    }
}
