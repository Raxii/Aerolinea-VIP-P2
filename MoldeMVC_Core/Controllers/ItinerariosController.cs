using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;
namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class ItinerariosController : Controller
    {
        private readonly MongoDbContext _context;
        public ItinerariosController(MongoDbContext context) { _context = context; }
        private static string ListaJson<T>(IEnumerable<T> lista) { var a = new BsonArray(); foreach (var x in lista) a.Add(x.ToBsonDocument()); return a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }); }
        private static List<T> LeerLista<T>(string? json) => string.IsNullOrWhiteSpace(json) ? new List<T>() : BsonSerializer.Deserialize<List<T>>(json);
        private void Preparar(Itinerario m, string? json = null) => ViewBag.TramosJson = json ?? ListaJson(m.Tramos);
        public IActionResult Index(string? buscar) { var d = _context.Itinerarios.Find(x => true).ToList(); if (!string.IsNullOrWhiteSpace(buscar)) d = d.Where(x => x.Referencia.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList(); ViewBag.Buscar = buscar; return View(d); }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Itinerarios.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpGet] public IActionResult Create() { var m = new Itinerario(); Preparar(m, "[]"); return View(m); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(Itinerario modelo, string? tramosJson)
        {
            try { modelo.Tramos = LeerLista<TramoItinerario>(tramosJson); } catch { ModelState.AddModelError("tramosJson", "El JSON de tramos no es válido. Los vueloId deben usar formato ObjectId extendido."); }
            if (!ModelState.IsValid) { Preparar(modelo, tramosJson); return View(modelo); }
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Itinerarios.InsertOne(modelo); TempData["Mensaje"] = "Itinerario creado correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar el itinerario. Revise la referencia y el ID SQL."); Preparar(modelo, tramosJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Itinerarios.Find(x => x.Id == oid).FirstOrDefault(); if (d == null) return NotFound(); Preparar(d); return View(d); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Edit(string id, Itinerario modelo, string? tramosJson)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound();
            try { modelo.Tramos = LeerLista<TramoItinerario>(tramosJson); } catch { ModelState.AddModelError("tramosJson", "El JSON de tramos no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, tramosJson); return View(modelo); }
            try { modelo.Id = oid; var r = _context.Itinerarios.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Itinerario actualizado correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar el itinerario."); Preparar(modelo, tramosJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Itinerarios.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Itinerarios.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Itinerario eliminado correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
