using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly MongoDbContext _context;

        public ReservasController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? buscar)
        {
            var datos = _context.Reservas.Find(documento => true).ToList();
            if (!string.IsNullOrWhiteSpace(buscar))
                datos = datos.Where(x => (x.CodigoReserva ?? "").Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.UsuarioId.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList();
            ViewBag.Buscar = buscar;
            return View(datos);
        }

        public IActionResult Details(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            var dato = _context.Reservas.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Reserva { FechaCreacion = DateTime.Now, Estado = "PENDIENTE" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                modelo.Id = ObjectId.GenerateNewId();
                _context.Reservas.InsertOne(modelo);
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
            var dato = _context.Reservas.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Reserva modelo)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            if (!ModelState.IsValid) return View(modelo);
            try
            {
                modelo.Id = objectId;
                var resultado = _context.Reservas.ReplaceOne(x => x.Id == objectId, modelo);
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
            var dato = _context.Reservas.Find(x => x.Id == objectId).FirstOrDefault();
            if (dato == null) return NotFound();
            return View(dato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return NotFound();
            _context.Reservas.DeleteOne(x => x.Id == objectId);
            TempData["Mensaje"] = "Registro eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
