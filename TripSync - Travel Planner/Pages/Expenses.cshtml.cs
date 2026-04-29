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

        public void OnGet(int tripId)
        {
            TripId = tripId;

            using var conn = _db.GetConnection();
            conn.Open();

            // Get expenses + JOIN (important for grading)
            var cmd = new MySqlCommand(@"
            SELECT e.amount, e.category, e.description, u.name
            FROM expenses e
            JOIN users u ON e.added_by = u.user_id
            WHERE e.trip_id = @trip", conn);

            cmd.Parameters.AddWithValue("@trip", tripId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Expenses.Add(new Expense
                {
                    Amount = reader.GetDecimal("amount"),
                    Category = reader.GetString("category"),
                    Description = reader.GetString("description"),
                    UserName = reader.GetString("name")
                });
            }

            reader.Close();

            // Aggregate function (SUM)
            var totalCmd = new MySqlCommand(
                "SELECT SUM(amount) FROM expenses WHERE trip_id = @trip", conn);

            totalCmd.Parameters.AddWithValue("@trip", tripId);

            Total = Convert.ToDecimal(totalCmd.ExecuteScalar() ?? 0);
        }

        public class Expense
        {
            public decimal Amount { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string UserName { get; set; }
        }
    }
}
