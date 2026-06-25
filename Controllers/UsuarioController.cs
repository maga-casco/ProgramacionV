using BCrypt;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProgramacionV.Models;
using System.Collections;

namespace ProgramacionV.Controllers
{
    public class UsuarioController : Controller
    {
        private IConfiguration _configuration;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Index(UsuarioViewModel model)
        {
            string connectionString = _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query = "SELECT Contrasena FROM Usuarios WHERE Usuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Usuario", model.Username);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        string hashGuardado = resultado.ToString();

                        bool passwordValida = BCrypt.Net.BCrypt.Verify(model.Password,hashGuardado);

                        if (passwordValida)
                        {
                            HttpContext.Session.SetString("UsuarioLogueado",model.Username);

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";

            return View(model);
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            var usuario = HttpContext.Session.GetString("UsuarioLogueado");
            if (usuario == null)
            {
                return RedirectToAction("Index", "Usuario");
            } else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            string passwordCrypt =
                BCrypt.Net.BCrypt.HashPassword(model.Password);


            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");


            using (SqlConnection sqlConnection =
                   new SqlConnection(connectionString))
            {
                sqlConnection.Open();


                string query = @"INSERT INTO dbo.Usuarios 
                        (Usuario, Contrasena) 
                        VALUES (@Usuario, @Contrasena)";


                using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Usuario", model.Username);
                    cmd.Parameters.AddWithValue("@Contrasena", passwordCrypt);


                    int count = cmd.ExecuteNonQuery();


                    if (count > 0)
                    {
                        TempData["Mensaje"] =
                            "Usuario agregado correctamente";

                        return RedirectToAction("Create");
                    }
                }
            }


            return View(model);
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // Cerrar Sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Usuario");
        }

    }
}
