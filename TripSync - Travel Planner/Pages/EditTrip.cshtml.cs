using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class EditTripModel : PageModel
    {
        private readonly DbHelper _db;

        public EditTripModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty] public int TripId { get; set; }
        [BindProperty, Required, StringLength(100)] public string TripName { get; set; } = string.Empty;
        [BindProperty, Required, StringLength(100)] public string Destination { get; set; } = string.Empty;
        [BindProperty] public DateTime StartDate { get; set; }
        [BindProperty] public DateTime EndDate { get; set; }

        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripOwner(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT trip_id, trip_name, destination, start_date, end_date
                FROM trips
                WHERE trip_id = @trip", conn);
            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return RedirectToPage("/Dashboard");

            TripId = reader.GetInt32("trip_id");
            TripName = reader.GetString("trip_name");
            Destination = reader.GetString("destination");
            StartDate = reader.GetDateTime("start_date");
            EndDate = reader.GetDateTime("end_date");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripOwner(TripId, userId.Value))
                return RedirectToPage("/Dashboard");

            if (EndDate < StartDate)
                ModelState.AddModelError(nameof(EndDate), "End date must be on or after the start date.");

            if (!ModelState.IsValid)
                return Page();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                UPDATE trips
                SET trip_name = @name,
                    destination = @destination,
                    start_date = @start,
                    end_date = @end
                WHERE trip_id = @trip", conn);
            cmd.Parameters.AddWithValue("@name", TripName);
            cmd.Parameters.AddWithValue("@destination", Destination);
            cmd.Parameters.AddWithValue("@start", StartDate);
            cmd.Parameters.AddWithValue("@end", EndDate);
            cmd.Parameters.AddWithValue("@trip", TripId);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/TripDetails", new { tripId = TripId });
        }
    }
}
