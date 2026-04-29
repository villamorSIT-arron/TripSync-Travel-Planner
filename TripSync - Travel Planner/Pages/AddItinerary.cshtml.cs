using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
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

        [BindProperty] public string Title { get; set; }
        [BindProperty] public DateTime Start { get; set; }
        [BindProperty] public DateTime End { get; set; }

        public IActionResult OnPost(int tripId)
        {
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
