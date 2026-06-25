using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProgramacionV.Models;

namespace ProgramacionV.Controllers
{
    public class SocioController : Controller
    {
        private IConfiguration _configuration;

        public SocioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ActionResult Create()
        {
            var usuario =
                HttpContext.Session.GetString("UsuarioLogueado");

            if (usuario == null)
            {
                return RedirectToAction("Index", "Usuario");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(SocioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                sqlConnection.Open();

                // Verificar si el DNI ya existe
                string verificarDni =
                    @"SELECT COUNT(*) FROM Socios WHERE DNI = @DNI";

                using (SqlCommand cmdVerificar =
                    new SqlCommand(verificarDni, sqlConnection))
                {
                    cmdVerificar.Parameters.AddWithValue("@DNI", model.DNI);

                    int existe = (int)cmdVerificar.ExecuteScalar();

                    if (existe > 0)
                    {
                        ModelState.AddModelError(
                            "DNI",
                            "Ya existe un socio registrado con ese DNI.");

                        return View(model);
                    }
                }

                string query = @"INSERT INTO Socios(Nombre, Apellido, DNI, FechaNacimiento, Telefono, Email, Direccion, Activo)
                                    VALUES (@Nombre, @Apellido, @DNI, @FechaNacimiento, @Telefono, @Email, @Direccion, @Activo)";

                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", model.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", model.DNI);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@Telefono", model.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                    cmd.Parameters.AddWithValue("@Direccion", model.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@Activo", model.Activo);

                    int filas = cmd.ExecuteNonQuery();

                    if (filas > 0)
                    {
                        TempData["Mensaje"] = "Socio agregado correctamente";

                        return RedirectToAction("Index");
                    }
                }
            }

            return View(model);
        }

        public ActionResult Index(string buscar)
        {
            List<SocioViewModel> lista =
                new List<SocioViewModel>();

            string connectionString = _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query;


                if (string.IsNullOrEmpty(buscar))
                {
                    query = @"SELECT * FROM Socios";
                }
                else
                {
                    query = @"SELECT *
                              FROM Socios
                              WHERE Nombre LIKE @Buscar
                              OR Apellido LIKE @Buscar
                              OR DNI LIKE @Buscar";
                }

                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {
                    if (!string.IsNullOrEmpty(buscar))
                    {
                        cmd.Parameters.AddWithValue("@Buscar",
                            "%" + buscar + "%");
                    }

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        lista.Add(new SocioViewModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            DNI = dr["DNI"].ToString(),
                            FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                            Telefono = dr["Telefono"].ToString(),
                            Email = dr["Email"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }

            return View(lista);
        }

        public ActionResult Edit(int id)
        {

            var usuario =
                HttpContext.Session.GetString("UsuarioLogueado");

            if (usuario == null)
            {
                return RedirectToAction("Index", "Usuario");
            }

            SocioViewModel socio = new SocioViewModel();

            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                string query =
                    @"SELECT * FROM Socios WHERE Id = @Id";

                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        socio.Id = Convert.ToInt32(dr["Id"]);
                        socio.Nombre = dr["Nombre"].ToString();
                        socio.Apellido = dr["Apellido"].ToString();
                        socio.DNI = dr["DNI"].ToString();
                        socio.FechaNacimiento =
                            Convert.ToDateTime(dr["FechaNacimiento"]);
                        socio.Telefono =
                            dr["Telefono"].ToString();
                        socio.Email =
                            dr["Email"].ToString();
                        socio.Direccion = dr["Direccion"].ToString();
                        socio.Activo = Convert.ToBoolean(dr["Activo"]);
                    }
                }
            }

            return View(socio);
        }

        [HttpPost]
        public ActionResult Edit(SocioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");

            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Verificar si el DNI ya pertenece a otro socio
                string verificarDni =
                    @"SELECT COUNT(*)
              FROM Socios
              WHERE DNI = @DNI
              AND Id <> @Id";

                using (SqlCommand cmdVerificar =
                    new SqlCommand(verificarDni, sqlConnection))
                {
                    cmdVerificar.Parameters.AddWithValue("@DNI", model.DNI);
                    cmdVerificar.Parameters.AddWithValue("@Id", model.Id);

                    int existe =
                        (int)cmdVerificar.ExecuteScalar();

                    if (existe > 0)
                    {
                        ModelState.AddModelError(
                            "DNI",
                            "Ya existe otro socio con ese DNI.");

                        return View(model);
                    }
                }

                string query =
                    @"UPDATE Socios
              SET Nombre = @Nombre,
                  Apellido = @Apellido,
                  DNI = @DNI,
                  FechaNacimiento = @FechaNacimiento,
                  Telefono = @Telefono,
                  Email = @Email,
                  Direccion = @Direccion,
                    Activo = @Activo
              WHERE Id = @Id";

                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", model.Apellido);
                    cmd.Parameters.AddWithValue("@DNI", model.DNI);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", model.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@Telefono", model.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                    cmd.Parameters.AddWithValue("@Direccion", model.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@Activo", model.Activo);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] =
                "Los datos del socio fueron actualizados correctamente.";

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var usuario =
            HttpContext.Session.GetString("UsuarioLogueado");

            if (usuario == null)
            {
                return RedirectToAction("Index", "Usuario");
            }

            SocioViewModel socio = new SocioViewModel();

            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");


            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {
                sqlConnection.Open();


                string query =
                @"SELECT * FROM Socios WHERE Id = @Id";


                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@Id", id);


                    SqlDataReader dr =
                        cmd.ExecuteReader();


                    if (dr.Read())
                    {
                        socio.Id = Convert.ToInt32(dr["Id"]);
                        socio.Nombre = dr["Nombre"].ToString();
                        socio.Apellido = dr["Apellido"].ToString();
                        socio.DNI = dr["DNI"].ToString();
                        socio.Activo = Convert.ToBoolean(dr["Activo"]);
                    }
                }
            }


            return View(socio);
        }

        [HttpPost]
        public ActionResult DeleteConfirmado(int id)
        {

            string connectionString =
                _configuration.GetConnectionString("ProgramacionV");


            using (SqlConnection sqlConnection =
                new SqlConnection(connectionString))
            {

                sqlConnection.Open();


                string query =
                @"DELETE FROM Socios WHERE Id=@Id";


                using (SqlCommand cmd =
                    new SqlCommand(query, sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }


            TempData["Mensaje"] =
                "Socio eliminado correctamente";


            return RedirectToAction("Index");
        }
    }
}
