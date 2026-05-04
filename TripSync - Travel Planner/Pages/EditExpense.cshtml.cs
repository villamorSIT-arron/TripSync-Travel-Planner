using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class EditExpenseModel : PageModel
    {
        private readonly DbHelper _db;

        public EditExpenseModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty] public int ExpenseId { get; set; }
        [BindProperty] public int TripId { get; set; }
        [BindProperty, Range(0.01, 999999999)] public decimal Amount { get; set; }
        [BindProperty, StringLength(50)] public string Category { get; set; } = string.Empty;
        [BindProperty, StringLength(255)] public string Description { get; set; } = string.Empty;

        public IActionResult OnGet(int expenseId, int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT expense_id, trip_id, amount, category, description
                FROM expenses
                WHERE expense_id = @expense AND trip_id = @trip", conn);
            cmd.Parameters.AddWithValue("@expense", expenseId);
            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return RedirectToPage("/Expenses", new { tripId });

            ExpenseId = reader.GetInt32("expense_id");
            TripId = reader.GetInt32("trip_id");
            Amount = reader.GetDecimal("amount");
            Category = reader.IsDBNull(reader.GetOrdinal("category")) ? string.Empty : reader.GetString("category");
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description");

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(TripId, userId.Value))
                return RedirectToPage("/Dashboard");

            if (!ModelState.IsValid)
                return Page();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                UPDATE expenses
                SET amount = @amount,
                    category = @category,
                    description = @description
                WHERE expense_id = @expense AND trip_id = @trip", conn);
            cmd.Parameters.AddWithValue("@amount", Amount);
            cmd.Parameters.AddWithValue("@category", Category);
            cmd.Parameters.AddWithValue("@description", Description);
            cmd.Parameters.AddWithValue("@expense", ExpenseId);
            cmd.Parameters.AddWithValue("@trip", TripId);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Expenses", new { tripId = TripId });
        }
    }
}
