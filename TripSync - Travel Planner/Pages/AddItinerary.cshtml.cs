using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class AddItineraryModel : PageModel
    {
        private readonly DbHelper _db;

        public AddItineraryModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty, Required, StringLength(100)] public string Title { get; set; } = string.Empty;
        [BindProperty] public DateTime Start { get; set; }
        [BindProperty] public DateTime End { get; set; }

        [BindProperty] public int TripId { get; set; }

        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            TripId = tripId;
            return Page();
        }

        public IActionResult OnPost(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            if (End < Start)
                ModelState.AddModelError(nameof(End), "End time must be on or after the start time.");

            if (!ModelState.IsValid)
            {
                TripId = tripId;
                return Page();
            }

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                @"INSERT INTO itinerary (trip_id, title, start_datetime, end_datetime)
              VALUES (@trip, @title, @start, @end)", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);
            cmd.Parameters.AddWithValue("@title", Title);
            cmd.Parameters.AddWithValue("@start", Start);
            cmd.Parameters.AddWithValue("@end", End);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/TripDetails", new { tripId });
        }
    }
}
