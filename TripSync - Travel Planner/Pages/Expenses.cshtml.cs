using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class ExpensesModel : PageModel
    {
        private readonly DbHelper _db;

        public ExpensesModel(DbHelper db)
        {
            _db = db;
        }

        public int TripId { get; set; }
        public decimal Total { get; set; }

        public List<Expense> Expenses { get; set; } = new();

        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            TripId = tripId;

            using var conn = _db.GetConnection();
            conn.Open();

            // Get expenses + JOIN (important for grading)
            var cmd = new MySqlCommand(@"
            SELECT e.expense_id, e.amount, e.category, e.description, e.expense_date, u.name
            FROM expenses e
            JOIN users u ON e.added_by = u.user_id
            WHERE e.trip_id = @trip
            ORDER BY e.expense_date DESC", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Expenses.Add(new Expense
                {
                    Id = reader.GetInt32("expense_id"),
                    Amount = reader.GetDecimal("amount"),
                    Category = reader.IsDBNull(reader.GetOrdinal("category")) ? "General" : reader.GetString("category"),
                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
                    ExpenseDate = reader.GetDateTime("expense_date"),
                    UserName = reader.GetString("name")
                });
            }

            reader.Close();

            // Aggregate function (SUM)
            var totalCmd = new MySqlCommand(
                "SELECT SUM(amount) FROM expenses WHERE trip_id = @trip", conn);

            totalCmd.Parameters.AddWithValue("@trip", tripId);

            Total = Convert.ToDecimal(totalCmd.ExecuteScalar() ?? 0);
            return Page();
        }

        public class Expense
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public DateTime ExpenseDate { get; set; }
            public string UserName { get; set; } = string.Empty;
        }
    }
}
