using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DbHelper _db;

        public LoginModel(DbHelper db)
        {
            _db = db;
        }

        [BindProperty]
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT user_id, password FROM users WHERE email=@email", conn);

            cmd.Parameters.AddWithValue("@email", Email);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var userId = reader.GetInt32("user_id");
                var storedPassword = reader.GetString("password");

                if (PasswordHasher.Verify(Password, storedPassword))
                {
                    reader.Close();

                    if (PasswordHasher.NeedsUpgrade(storedPassword))
                    {
                        var updateCmd = new MySqlCommand(
                            "UPDATE users SET password = @password WHERE user_id = @user", conn);
                        updateCmd.Parameters.AddWithValue("@password", PasswordHasher.Hash(Password));
                        updateCmd.Parameters.AddWithValue("@user", userId);
                        updateCmd.ExecuteNonQuery();
                    }

                    HttpContext.Session.SetInt32("UserId", userId);
                    return RedirectToPage("/Dashboard");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }
    }
}
