using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class DeleteTripModel : PageModel
    {
        private readonly DbHelper _db;

        public DeleteTripModel(DbHelper db)
        {
            _db = db;
        }

        public IActionResult OnGet(int tripId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM trips WHERE trip_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", tripId);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Dashboard");
        }
    }
}
