using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class TripDetailsModel : PageModel
    {
        private readonly DbHelper _db;

        public TripDetailsModel(DbHelper db)
        {
            _db = db;
        }

        public int TripId { get; set; }
        public decimal TotalExpense { get; set; }
        public int TotalActivities { get; set; }

        public void OnGet(int tripId)
        {
            TripId = tripId;

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd1 = new MySqlCommand(
                "SELECT IFNULL(SUM(amount),0) FROM expenses WHERE trip_id=@id", conn);
            cmd1.Parameters.AddWithValue("@id", tripId);
            TotalExpense = Convert.ToDecimal(cmd1.ExecuteScalar());

            var cmd2 = new MySqlCommand(
                "SELECT COUNT(*) FROM itinerary WHERE trip_id=@id", conn);
            cmd2.Parameters.AddWithValue("@id", tripId);
            TotalActivities = Convert.ToInt32(cmd2.ExecuteScalar());
        }
    }
}
