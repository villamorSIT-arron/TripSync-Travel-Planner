using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class ItineraryModel : PageModel
    {
        private readonly DbHelper _db;

        public ItineraryModel(DbHelper db)
        {
            _db = db;
        }

        public int TripId { get; set; }
        public List<Item> Items { get; set; } = new();

        public void OnGet(int tripId)
        {
            TripId = tripId;

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT title, start_datetime, end_datetime FROM itinerary WHERE trip_id=@trip ORDER BY start_datetime", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Items.Add(new Item
                {
                    Title = reader.GetString("title"),
                    Start = reader.GetDateTime("start_datetime"),
                    End = reader.GetDateTime("end_datetime")
                });
            }
        }

        public class Item
        {
            public string Title { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
