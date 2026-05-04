using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
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

        [BindProperty, Required, StringLength(100)] public string TripName { get; set; } = string.Empty;
        [BindProperty, Required, StringLength(100)] public string Destination { get; set; } = string.Empty;
        [BindProperty] public DateTime StartDate { get; set; }
        [BindProperty] public DateTime EndDate { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToPage("/Login");

            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (EndDate < StartDate)
                ModelState.AddModelError(nameof(EndDate), "End date must be on or after the start date.");

            if (!ModelState.IsValid)
                return Page();

            using var conn = _db.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            var cmd = new MySqlCommand(
                @"INSERT INTO trips (trip_name, destination, start_date, end_date, created_by)
              VALUES (@name, @dest, @start, @end, @user)", conn, transaction);

            cmd.Parameters.AddWithValue("@name", TripName);
            cmd.Parameters.AddWithValue("@dest", Destination);
            cmd.Parameters.AddWithValue("@start", StartDate);
            cmd.Parameters.AddWithValue("@end", EndDate);
            cmd.Parameters.AddWithValue("@user", userId.Value);

            cmd.ExecuteNonQuery();

            long tripId = cmd.LastInsertedId;

            var memberCmd = new MySqlCommand(
                "INSERT INTO trip_members (trip_id, user_id, role) VALUES (@trip, @user, 'owner')", conn, transaction);

            memberCmd.Parameters.AddWithValue("@trip", tripId);
            memberCmd.Parameters.AddWithValue("@user", userId.Value);

            memberCmd.ExecuteNonQuery();
            transaction.Commit();

            return RedirectToPage("/Dashboard");
        }
    }
}
