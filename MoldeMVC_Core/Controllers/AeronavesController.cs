
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
using System.Text.RegularExpressions;

namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class AeronavesController : Controller
    {
        private readonly MongoDbContext _context;

        public AeronavesController(MongoDbContext context)
        {
            _context = context;
        }

        //*************** CONVERTIR ASIENTOS A JSON ***************
        private static string ListaJson<T>(IEnumerable<T> lista)
        {
            var array = new BsonArray();

            foreach (var elemento in lista)
            {
                array.Add(elemento.ToBsonDocument());
            }

            return array.ToJson(
                new MongoDB.Bson.IO.JsonWriterSettings
                {
                    Indent = true
                });
        }

        //*************** LEER ASIENTOS DESDE JSON ****************
        private static List<T> LeerLista<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return BsonSerializer.Deserialize<List<T>>(json)
                   ?? new List<T>();
        }

        //*************** PREPARAR FORMULARIO *********************
        private void Preparar(Aeronave modelo, string? json = null)
        {
            ViewBag.AsientosJson = json ??
                ListaJson(modelo.Asientos ?? new List<AsientoAeronave>());
        }

        //*************** VALIDACIONES BÁSICAS ********************
        private void ValidarAeronave(Aeronave modelo)
        {
            modelo.Matricula =
                (modelo.Matricula ?? "").Trim().ToUpperInvariant();

            modelo.Estado =
                (modelo.Estado ?? "").Trim();

            if (string.IsNullOrWhiteSpace(modelo.Matricula))
            {
                ModelState.AddModelError(
                    nameof(Aeronave.Matricula),
                    "Ingrese la matrícula de la aeronave.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Estado))
            {
                ModelState.AddModelError(
                    nameof(Aeronave.Estado),
                    "Ingrese el estado de la aeronave.");
            }

            if (modelo.IdSql <= 0)
            {
                ModelState.AddModelError(
                    nameof(Aeronave.IdSql),
                    "El ID SQL debe ser mayor que cero.");
            }
        }

        //*************** LISTAR Y BUSCAR *************************
        public async Task<IActionResult> Index(string? buscar)
        {
            var filtrosBuilder = Builders<Aeronave>.Filter;

            FilterDefinition<Aeronave> filtro =
                filtrosBuilder.Empty;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                var expresion = new BsonRegularExpression(
                    Regex.Escape(buscar), "i");

                var filtros = new List<FilterDefinition<Aeronave>>
                {
                    filtrosBuilder.Regex(
                        x => x.Matricula, expresion),

                    filtrosBuilder.Regex(
                        x => x.Estado, expresion),

                    filtrosBuilder.Regex(
                        x => x.Modelo.Nombre, expresion),

                    filtrosBuilder.Regex(
                        x => x.Modelo.Fabricante, expresion)
                };

                if (int.TryParse(buscar, out int idSql))
                {
                    filtros.Add(
                        filtrosBuilder.Eq(
                            x => x.IdSql, idSql));
                }

                filtro = filtrosBuilder.Or(filtros);
            }

            var datos = await _context.Aeronaves
                .Find(filtro)
                .SortBy(x => x.Matricula)
                .ToListAsync();

            ViewBag.Buscar = buscar;

            return View(datos);
        }

        //*************** DETALLES ********************************
        public async Task<IActionResult> Details(string id)
        {
            if (!ObjectId.TryParse(id, out var oid))
                return NotFound();

            var dato = await _context.Aeronaves
                .Find(x => x.Id == oid)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            return View(dato);
        }

        //*************** CREAR - GET *****************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            var modelo = new Aeronave();

            Preparar(modelo, "[]");

            return View(modelo);
        }

        //*************** CREAR - POST ****************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Aeronave modelo,
            string? asientosJson)
        {
            try
            {
                modelo.Asientos =
                    LeerLista<AsientoAeronave>(asientosJson);
            }
            catch
            {
                ModelState.AddModelError(
                    "asientosJson",
                    "El JSON de asientos no es válido.");
            }

            ValidarAeronave(modelo);

            if (!ModelState.IsValid)
            {
                Preparar(modelo, asientosJson);
                return View(modelo);
            }

            try
            {
                var builder = Builders<Aeronave>.Filter;

                // Matrícula sin duplicados
                var filtroMatricula = builder.Regex(
                    x => x.Matricula,
                    new BsonRegularExpression(
                        "^" + Regex.Escape(modelo.Matricula) + "$",
                        "i"));

                var duplicadoMatricula =
                    await _context.Aeronaves
                        .Find(filtroMatricula)
                        .FirstOrDefaultAsync();

                if (duplicadoMatricula != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeronave.Matricula),
                        "Ya existe una aeronave con esta matrícula.");
                }

                // ID SQL sin duplicados
                var duplicadoId =
                    await _context.Aeronaves
                        .Find(x => x.IdSql == modelo.IdSql)
                        .FirstOrDefaultAsync();

                if (duplicadoId != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeronave.IdSql),
                        "Ya existe una aeronave con este ID SQL.");
                }

                if (!ModelState.IsValid)
                {
                    Preparar(modelo, asientosJson);
                    return View(modelo);
                }

                modelo.Id = ObjectId.GenerateNewId();

                await _context.Aeronaves.InsertOneAsync(modelo);

                TempData["Mensaje"] =
                    "Aeronave creada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo guardar la aeronave.");

                Preparar(modelo, asientosJson);

                return View(modelo);
            }
        }

        //*************** EDITAR - GET ****************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!ObjectId.TryParse(id, out var oid))
                return NotFound();

            var dato = await _context.Aeronaves
                .Find(x => x.Id == oid)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            Preparar(dato);

            return View(dato);
        }

        //*************** EDITAR - POST ***************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            Aeronave modelo,
            string? asientosJson)
        {
            if (!ObjectId.TryParse(id, out var oid))
                return NotFound();

            modelo.Id = oid;

            try
            {
                modelo.Asientos =
                    LeerLista<AsientoAeronave>(asientosJson);
            }
            catch
            {
                ModelState.AddModelError(
                    "asientosJson",
                    "El JSON de asientos no es válido.");
            }

            ValidarAeronave(modelo);

            if (!ModelState.IsValid)
            {
                Preparar(modelo, asientosJson);
                return View(modelo);
            }

            try
            {
                var builder = Builders<Aeronave>.Filter;

                // Verificar matrícula en otra aeronave
                var filtroMatricula = builder.And(
                    builder.Ne(x => x.Id, oid),
                    builder.Regex(
                        x => x.Matricula,
                        new BsonRegularExpression(
                            "^" + Regex.Escape(modelo.Matricula) + "$",
                            "i"))
                );

                var duplicadoMatricula =
                    await _context.Aeronaves
                        .Find(filtroMatricula)
                        .FirstOrDefaultAsync();

                if (duplicadoMatricula != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeronave.Matricula),
                        "La matrícula pertenece a otra aeronave.");
                }

                // Verificar ID SQL en otra aeronave
                var filtroId = builder.And(
                    builder.Ne(x => x.Id, oid),
                    builder.Eq(x => x.IdSql, modelo.IdSql)
                );

                var duplicadoId =
                    await _context.Aeronaves
                        .Find(filtroId)
                        .FirstOrDefaultAsync();

                if (duplicadoId != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeronave.IdSql),
                        "El ID SQL pertenece a otra aeronave.");
                }

                if (!ModelState.IsValid)
                {
                    Preparar(modelo, asientosJson);
                    return View(modelo);
                }

                var resultado =
                    await _context.Aeronaves.ReplaceOneAsync(
                        x => x.Id == oid,
                        modelo);

                if (resultado.MatchedCount == 0)
                    return NotFound();

                TempData["Mensaje"] =
                    "Aeronave actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar la aeronave.");

                Preparar(modelo, asientosJson);

                return View(modelo);
            }
        }

        //*************** ELIMINAR - GET **************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var oid))
                return NotFound();

            var dato = await _context.Aeronaves
                .Find(x => x.Id == oid)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            return View(dato);
        }

        //*************** ELIMINAR - POST *************************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!ObjectId.TryParse(id, out var oid))
                return NotFound();

            try
            {
                var resultado =
                    await _context.Aeronaves.DeleteOneAsync(
                        x => x.Id == oid);

                if (resultado.DeletedCount == 0)
                    return NotFound();

                TempData["Mensaje"] =
                    "Aeronave eliminada correctamente.";
            }
            catch (MongoException)
            {
                TempData["Mensaje"] =
                    "No se pudo eliminar la aeronave.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
