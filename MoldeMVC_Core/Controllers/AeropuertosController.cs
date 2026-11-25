using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;

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

        public IActionResult Index(string? buscar)
        {
            var datos = _context.Aeropuertos.Find(documento => true).ToList();
            if (!string.IsNullOrWhiteSpace(buscar))
                datos = datos.Where(x => x.CodigoIATA.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Localidad.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList();
            ViewBag.Buscar = buscar;
            return View(datos);
        }

        public IActionResult Details(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            var dato = _context.Aeropuertos.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Aeropuerto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Aeropuerto modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                modelo.Id = ObjectId.GenerateNewId();
                _context.Aeropuertos.InsertOne(modelo);
                TempData["Mensaje"] = "Registro creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar. Verifique los datos y que no exista un identificador duplicado.");
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            var dato = _context.Aeropuertos.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Aeropuerto modelo)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                modelo.Id = objectId;
                var resultado = _context.Aeropuertos.ReplaceOne(x => x.Id == objectId, modelo);
                if (resultado.MatchedCount == 0) return NotFound();
                TempData["Mensaje"] = "Registro actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar. Verifique los datos y que no exista un identificador duplicado.");
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            var dato = _context.Aeropuertos.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            _context.Aeropuertos.DeleteOne(x => x.Id == objectId);
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
