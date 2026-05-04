using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using TripSync___Travel_Planner.Data;

namespace TripSync___Travel_Planner.Pages
{
    public class DeleteTripModel : PageModel
    {
        private readonly DbHelper _db;

        public DeleteTripModel(DbHelper db)
        {
            _db = db;
        }

        public IActionResult OnPost(int tripId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            if (!_db.IsTripOwner(tripId, userId.Value))
                return RedirectToPage("/Dashboard");

            using var conn = _db.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            var deleteExpensesCmd = new MySqlCommand(
                "DELETE FROM expenses WHERE trip_id = @id", conn, transaction);
            deleteExpensesCmd.Parameters.AddWithValue("@id", tripId);
            deleteExpensesCmd.ExecuteNonQuery();

            var deleteItineraryCmd = new MySqlCommand(
                "DELETE FROM itinerary WHERE trip_id = @id", conn, transaction);
            deleteItineraryCmd.Parameters.AddWithValue("@id", tripId);
            deleteItineraryCmd.ExecuteNonQuery();

            var deleteMembersCmd = new MySqlCommand(
                "DELETE FROM trip_members WHERE trip_id = @id", conn, transaction);
            deleteMembersCmd.Parameters.AddWithValue("@id", tripId);
            deleteMembersCmd.ExecuteNonQuery();

            var cmd = new MySqlCommand("DELETE FROM trips WHERE trip_id = @id", conn, transaction);
            cmd.Parameters.AddWithValue("@id", tripId);

            cmd.ExecuteNonQuery();
            transaction.Commit();

            return RedirectToPage("/Dashboard");
        }
    }
}
