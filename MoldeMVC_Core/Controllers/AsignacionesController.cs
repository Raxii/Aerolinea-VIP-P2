using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class AsignacionesController : Controller
    {
        private readonly MongoDbContext _context;
        public AsignacionesController(MongoDbContext context) { _context = context; }
        private bool AsignarReferencias(Asignacion m, string? reservaId, string? pasajeroId, string? vueloId)
        {
            bool ok = true;
            if (!ObjectId.TryParse(reservaId, out var r)) { ModelState.AddModelError("reservaId", "ObjectId de reserva inválido."); ok = false; } else m.ReservaId = r;
            if (!ObjectId.TryParse(pasajeroId, out var p)) { ModelState.AddModelError("pasajeroId", "ObjectId de pasajero inválido."); ok = false; } else m.PasajeroId = p;
            if (!ObjectId.TryParse(vueloId, out var v)) { ModelState.AddModelError("vueloId", "ObjectId de vuelo inválido."); ok = false; } else m.VueloId = v;
            return ok;
        }
        public IActionResult Index(string? buscar) { var d = _context.Asignaciones.Find(x => true).ToList(); if (!string.IsNullOrWhiteSpace(buscar)) d = d.Where(x => x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.NumeroAsiento.ToString().Contains(buscar) || x.IdSql.ToString().Contains(buscar)).ToList(); ViewBag.Buscar = buscar; return View(d); }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Asignaciones.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpGet] public IActionResult Create() { return View(new Asignacion { FechaAsignacion = DateTime.Now, Estado = "PENDIENTE", CheckIn = new CheckIn { FechaRegistro = DateTime.Now, Estado = "PENDIENTE" } }); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(Asignacion modelo, string? reservaId, string? pasajeroId, string? vueloId, bool incluirCheckIn)
        {
            AsignarReferencias(modelo, reservaId, pasajeroId, vueloId);
            if (!incluirCheckIn) modelo.CheckIn = null;
            if (!ModelState.IsValid) return View(modelo);
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Asignaciones.InsertOne(modelo); TempData["Mensaje"] = "Asignación creada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar la asignación. Revise el ID SQL y las referencias."); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Asignaciones.Find(x => x.Id == oid).FirstOrDefault(); if (d == null) return NotFound(); d.CheckIn ??= new CheckIn(); ViewBag.ReservaId = d.ReservaId.ToString(); ViewBag.PasajeroId = d.PasajeroId.ToString(); ViewBag.VueloId = d.VueloId.ToString(); return View(d); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Edit(string id, Asignacion modelo, string? reservaId, string? pasajeroId, string? vueloId, bool incluirCheckIn)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound(); AsignarReferencias(modelo, reservaId, pasajeroId, vueloId); if (!incluirCheckIn) modelo.CheckIn = null;
            if (!ModelState.IsValid) { ViewBag.ReservaId = reservaId; ViewBag.PasajeroId = pasajeroId; ViewBag.VueloId = vueloId; return View(modelo); }
            try { modelo.Id = oid; var r = _context.Asignaciones.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Asignación actualizada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar la asignación."); ViewBag.ReservaId = reservaId; ViewBag.PasajeroId = pasajeroId; ViewBag.VueloId = vueloId; return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Asignaciones.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Asignaciones.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Asignación eliminada correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
