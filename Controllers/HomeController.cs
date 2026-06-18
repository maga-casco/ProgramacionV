using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProgramacionV.Models;
using System.Diagnostics;

namespace ProgramacionV.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            DashboardViewModel model =
                new DashboardViewModel();

            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Total Socios

                string querySocios =
                    "SELECT COUNT(*) FROM Socios";

                using (SqlCommand cmd =
                    new SqlCommand(querySocios, sqlConnection))
                {
                    model.TotalSocios =
                        Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Total Usuarios

                string queryUsuarios =
                    "SELECT COUNT(*) FROM Usuarios";

                using (SqlCommand cmd =
                    new SqlCommand(queryUsuarios, sqlConnection))
                {
                    model.TotalUsuarios =
                        Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            model.FechaActual =
                DateTime.Now;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]

        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
                });
        }
    }
}