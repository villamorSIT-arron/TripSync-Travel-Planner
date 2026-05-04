using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly DbHelper _db;

        public CalendarModel(DbHelper db)
        {
            _db = db;
        }

        public List<EventItem> Events { get; set; } = new();    

        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT title, start_datetime FROM itinerary WHERE trip_id=@trip", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Events.Add(new EventItem
                {
                    Title = reader.GetString("title"),
                    Start = reader.GetDateTime("start_datetime")
                });
            }

            return Page();
        }

        public class EventItem
        {
            public string Title { get; set; } = string.Empty;
            public DateTime Start { get; set; }
        }
    }
}
