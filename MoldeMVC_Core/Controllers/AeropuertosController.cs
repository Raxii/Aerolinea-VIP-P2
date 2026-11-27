
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
using System.Text.RegularExpressions;

namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class AeropuertosController : Controller
    {
        private readonly MongoDbContext _context;

        public AeropuertosController(MongoDbContext context)
        {
            _context = context;
        }

        //******************** LISTAR Y BUSCAR ********************
        public async Task<IActionResult> Index(string? buscar)
        {
            var filtro = Builders<Aeropuerto>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                var expresion = new BsonRegularExpression(
                    Regex.Escape(buscar), "i");

                var filtros = new List<FilterDefinition<Aeropuerto>>
                {
                    Builders<Aeropuerto>.Filter.Regex(
                        x => x.CodigoIATA, expresion),

                    Builders<Aeropuerto>.Filter.Regex(
                        x => x.Nombre, expresion),

                    Builders<Aeropuerto>.Filter.Regex(
                        x => x.Localidad, expresion)
                };

                if (int.TryParse(buscar, out int idSql))
                {
                    filtros.Add(
                        Builders<Aeropuerto>.Filter.Eq(
                            x => x.IdSql, idSql));
                }

                filtro = Builders<Aeropuerto>.Filter.Or(filtros);
            }

            var datos = await _context.Aeropuertos
                .Find(filtro)
                .SortBy(x => x.Nombre)
                .ToListAsync();

            ViewBag.Buscar = buscar;

            return View(datos);
        }

        //******************** DETALLES ********************
        public async Task<IActionResult> Details(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return NotFound();

            var dato = await _context.Aeropuertos
                .Find(x => x.Id == objectId)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            return View(dato);
        }

        //******************** CREAR - GET ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Aeropuerto { Activo = true });
        }

        //******************** CREAR - POST ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aeropuerto modelo)
        {
            ValidarAeropuerto(modelo);

            if (!ModelState.IsValid)
                return View(modelo);

            try
            {
                // Comprobar si el código IATA ya existe
                var filtroCodigo = Builders<Aeropuerto>.Filter.Regex(
                    x => x.CodigoIATA,
                    new BsonRegularExpression(
                        "^" + Regex.Escape(modelo.CodigoIATA) + "$",
                        "i"));

                var existente = await _context.Aeropuertos
                    .Find(filtroCodigo)
                    .FirstOrDefaultAsync();

                if (existente != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeropuerto.CodigoIATA),
                        "Ya existe un aeropuerto con este código IATA.");

                    return View(modelo);
                }

                modelo.Id = ObjectId.GenerateNewId();

                await _context.Aeropuertos.InsertOneAsync(modelo);

                TempData["Mensaje"] =
                    "Aeropuerto creado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo guardar el aeropuerto.");

                return View(modelo);
            }
        }

        //******************** EDITAR - GET ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return NotFound();

            var dato = await _context.Aeropuertos
                .Find(x => x.Id == objectId)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            return View(dato);
        }

        //******************** EDITAR - POST ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id, Aeropuerto modelo)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return NotFound();

            ValidarAeropuerto(modelo);

            if (!ModelState.IsValid)
                return View(modelo);

            try
            {
                // Evitar códigos IATA repetidos
                // en aeropuertos diferentes
                var filtroDuplicado =
                    Builders<Aeropuerto>.Filter.And(
                        Builders<Aeropuerto>.Filter.Ne(
                            x => x.Id, objectId),

                        Builders<Aeropuerto>.Filter.Regex(
                            x => x.CodigoIATA,
                            new BsonRegularExpression(
                                "^" + Regex.Escape(modelo.CodigoIATA) + "$",
                                "i"))
                    );

                var duplicado = await _context.Aeropuertos
                    .Find(filtroDuplicado)
                    .FirstOrDefaultAsync();

                if (duplicado != null)
                {
                    ModelState.AddModelError(
                        nameof(Aeropuerto.CodigoIATA),
                        "El código IATA pertenece a otro aeropuerto.");

                    return View(modelo);
                }

                modelo.Id = objectId;

                var resultado = await _context.Aeropuertos
                    .ReplaceOneAsync(
                        x => x.Id == objectId,
                        modelo);

                if (resultado.MatchedCount == 0)
                    return NotFound();

                TempData["Mensaje"] =
                    "Aeropuerto actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar el aeropuerto.");

                return View(modelo);
            }
        }

        //******************** ELIMINAR - GET ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return NotFound();

            var dato = await _context.Aeropuertos
                .Find(x => x.Id == objectId)
                .FirstOrDefaultAsync();

            if (dato == null)
                return NotFound();

            return View(dato);
        }

        //******************** ELIMINAR - POST ********************
        [Authorize(Roles = "Superusuario,Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return NotFound();

            try
            {
                var resultado = await _context.Aeropuertos
                    .DeleteOneAsync(x => x.Id == objectId);

                if (resultado.DeletedCount == 0)
                    return NotFound();

                TempData["Mensaje"] =
                    "Aeropuerto eliminado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                TempData["Mensaje"] =
                    "No se pudo eliminar el aeropuerto.";

                return RedirectToAction(nameof(Index));
            }
        }

        //******************** VALIDACIONES ********************
        private void ValidarAeropuerto(Aeropuerto modelo)
        {
            modelo.CodigoIATA =
                (modelo.CodigoIATA ?? "").Trim().ToUpperInvariant();

            modelo.Nombre =
                (modelo.Nombre ?? "").Trim();

            modelo.Localidad =
                (modelo.Localidad ?? "").Trim();

            if (!Regex.IsMatch(modelo.CodigoIATA, "^[A-Z]{3}$"))
            {
                ModelState.AddModelError(
                    nameof(Aeropuerto.CodigoIATA),
                    "El código IATA debe tener tres letras.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Nombre))
            {
                ModelState.AddModelError(
                    nameof(Aeropuerto.Nombre),
                    "Ingrese el nombre del aeropuerto.");
            }

            if (string.IsNullOrWhiteSpace(modelo.Localidad))
            {
                ModelState.AddModelError(
                    nameof(Aeropuerto.Localidad),
                    "Ingrese la localidad.");
            }
        }
    }
}
