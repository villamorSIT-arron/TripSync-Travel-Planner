using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;
    
namespace TripSync___Travel_Planner.Pages
{
    public class AddExpenseModel : PageModel
    {
        private readonly DbHelper _db;

        public AddExpenseModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty] public decimal Amount { get; set; }
        [BindProperty] public string Category { get; set; }
        [BindProperty] public string Description { get; set; }

        public int TripId { get; set; }

        public void OnGet(int tripId)
        {
            TripId = tripId;
        }

        public IActionResult OnPost(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                @"INSERT INTO expenses (trip_id, added_by, amount, category, description, expense_date)
              VALUES (@trip, @user, @amount, @cat, @desc, NOW())", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);
            cmd.Parameters.AddWithValue("@user", userId);
            cmd.Parameters.AddWithValue("@amount", Amount);
            cmd.Parameters.AddWithValue("@cat", Category);
            cmd.Parameters.AddWithValue("@desc", Description);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/TripDetails", new { tripId });
        }
    }
}
