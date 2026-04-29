using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
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
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand(
                "INSERT INTO users (name, email, password) VALUES (@name, @email, @password)", conn);

            cmd.Parameters.AddWithValue("@name", Name);
            cmd.Parameters.AddWithValue("@email", Email);
            cmd.Parameters.AddWithValue("@password", Password);

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Login");
        }
    }
}
