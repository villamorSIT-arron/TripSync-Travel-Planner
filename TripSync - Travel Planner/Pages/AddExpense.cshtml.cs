using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
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

        [BindProperty, Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        [BindProperty, StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [BindProperty, StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public int TripId { get; set; }

        // GET: Load page
        public IActionResult OnGet(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            TripId = tripId;
            return Page();
        }

        // POST: Submit expense
        public IActionResult OnPost(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripMember(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            if (!ModelState.IsValid)
            {
                TripId = tripId;
                return Page();
            }

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