using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using TripSync___Travel_Planner.Data;


namespace TripSync___Travel_Planner.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly DbHelper _db;

        public RegisterModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty]
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            using var conn = _db.GetConnection();
            conn.Open();

            var existingUserCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM users WHERE email = @email", conn);
            existingUserCmd.Parameters.AddWithValue("@email", Email);

            if (Convert.ToInt32(existingUserCmd.ExecuteScalar()) > 0)
            {
                ModelState.AddModelError(nameof(Email), "An account with this email already exists.");
                return Page();
            }

            var cmd = new MySqlCommand(
                "INSERT INTO users (name, email, password) VALUES (@name, @email, @password)", conn);

            cmd.Parameters.AddWithValue("@name", Name);
            cmd.Parameters.AddWithValue("@email", Email);
            cmd.Parameters.AddWithValue("@password", PasswordHasher.Hash(Password));

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Login");
        }
    }
}
