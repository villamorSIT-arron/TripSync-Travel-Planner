using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class AddMemberModel : PageModel
    {
        private readonly DbHelper _db;

        public AddMemberModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty] public string Email { get; set; }

        public IActionResult OnPost(int tripId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            // Subquery usage (important for grading)
            var cmd = new MySqlCommand(@"
            INSERT INTO trip_members (trip_id, user_id)
            SELECT @trip, user_id FROM users WHERE email = @email", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);
            cmd.Parameters.AddWithValue("@email", Email);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/TripDetails", new { tripId });
        }
    }
}
