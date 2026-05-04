using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class DeleteExpenseModel : PageModel
    {
        private readonly DbHelper _db;

        public DeleteExpenseModel(DbHelper db)
        {
            _db = db;
        }

        public IActionResult OnPost(int expenseId, int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(
                "DELETE FROM expenses WHERE expense_id = @expense AND trip_id = @trip", conn);
            cmd.Parameters.AddWithValue("@expense", expenseId);
            cmd.Parameters.AddWithValue("@trip", tripId);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Expenses", new { tripId });
        }
    }
}
