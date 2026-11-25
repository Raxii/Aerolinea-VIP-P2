using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class VuelosController : Controller
    {
        private readonly MongoDbContext _context;
        public VuelosController(MongoDbContext context) { _context = context; }
        private bool AsignarIds(Vuelo m, string? rutaId, string? origenAeropuertoId, string? destinoAeropuertoId, string? aeronaveId)
        {
            bool ok = true;
            if (!ObjectId.TryParse(rutaId, out var r)) { ModelState.AddModelError("rutaId", "Ingrese un ObjectId válido para la ruta."); ok = false; } else m.Ruta.RutaId = r;
            if (!ObjectId.TryParse(origenAeropuertoId, out var o)) { ModelState.AddModelError("origenAeropuertoId", "ObjectId del aeropuerto de origen inválido."); ok = false; } else m.Ruta.Origen.AeropuertoId = o;
            if (!ObjectId.TryParse(destinoAeropuertoId, out var d)) { ModelState.AddModelError("destinoAeropuertoId", "ObjectId del aeropuerto de destino inválido."); ok = false; } else m.Ruta.Destino.AeropuertoId = d;
            if (string.IsNullOrWhiteSpace(aeronaveId)) m.Aeronave = null;
            else if (!ObjectId.TryParse(aeronaveId, out var a)) { ModelState.AddModelError("aeronaveId", "ObjectId de aeronave inválido."); ok = false; }
            else if (m.Aeronave != null) m.Aeronave.AeronaveId = a;
            else { ModelState.AddModelError("aeronaveId", "Complete los datos de la aeronave o deje vacío su ObjectId."); ok = false; }
            return ok;
        }
        public IActionResult Index(string? buscar) { var d = _context.Vuelos.Find(x => true).ToList(); if (!string.IsNullOrWhiteSpace(buscar)) d = d.Where(x => x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Ruta.Origen.CodigoIATA.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Ruta.Destino.CodigoIATA.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList(); ViewBag.Buscar = buscar; return View(d); }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Vuelos.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpGet] public IActionResult Create() { return View(new Vuelo { FechaOperacion = DateTime.Now, Estado = "PROGRAMADO", Aeronave = new AeronaveVuelo() }); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(Vuelo modelo, string? rutaId, string? origenAeropuertoId, string? destinoAeropuertoId, string? aeronaveId)
        {
            AsignarIds(modelo, rutaId, origenAeropuertoId, destinoAeropuertoId, aeronaveId);
            if (!ModelState.IsValid) return View(modelo);
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Vuelos.InsertOne(modelo); TempData["Mensaje"] = "Vuelo creado correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar el vuelo. Revise el ID SQL y los ObjectId relacionados."); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Vuelos.Find(x => x.Id == oid).FirstOrDefault(); if (d == null) return NotFound(); d.Aeronave ??= new AeronaveVuelo(); ViewBag.RutaId = d.Ruta.RutaId.ToString(); ViewBag.OrigenId = d.Ruta.Origen.AeropuertoId.ToString(); ViewBag.DestinoId = d.Ruta.Destino.AeropuertoId.ToString(); ViewBag.AeronaveId = d.Aeronave?.AeronaveId.ToString() ?? ""; return View(d); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Edit(string id, Vuelo modelo, string? rutaId, string? origenAeropuertoId, string? destinoAeropuertoId, string? aeronaveId)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound(); AsignarIds(modelo, rutaId, origenAeropuertoId, destinoAeropuertoId, aeronaveId);
            if (!ModelState.IsValid) { ViewBag.RutaId = rutaId; ViewBag.OrigenId = origenAeropuertoId; ViewBag.DestinoId = destinoAeropuertoId; ViewBag.AeronaveId = aeronaveId; return View(modelo); }
            try { modelo.Id = oid; var r = _context.Vuelos.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Vuelo actualizado correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar el vuelo."); ViewBag.RutaId = rutaId; ViewBag.OrigenId = origenAeropuertoId; ViewBag.DestinoId = destinoAeropuertoId; ViewBag.AeronaveId = aeronaveId; return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Vuelos.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Vuelos.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Vuelo eliminado correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
