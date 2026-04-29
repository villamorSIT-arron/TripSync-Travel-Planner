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

        public void OnGet(int tripId)
        {
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
        }

        public class EventItem
        {
            public string Title { get; set; }
            public DateTime Start { get; set; }
        }
    }
}
