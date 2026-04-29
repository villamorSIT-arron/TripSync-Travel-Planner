using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
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
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "SELECT user_id FROM users WHERE email=@email AND password=@password", conn);

            cmd.Parameters.AddWithValue("@email", Email);
            cmd.Parameters.AddWithValue("@password", Password);

            var result = cmd.ExecuteScalar();

            if (result != null)
            {
                HttpContext.Session.SetInt32("UserId", Convert.ToInt32(result));
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
