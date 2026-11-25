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
    public class AeronavesController : Controller
    {
        private readonly MongoDbContext _context;
        public AeronavesController(MongoDbContext context) { _context = context; }

        private static string ListaJson<T>(IEnumerable<T> lista)
        {
            var array = new BsonArray();
            foreach (var elemento in lista) array.Add(elemento.ToBsonDocument());
            return array.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true });
        }
        private static List<T> LeerLista<T>(string? json) => string.IsNullOrWhiteSpace(json) ? new List<T>() : BsonSerializer.Deserialize<List<T>>(json);
        private void Preparar(Aeronave modelo, string? json = null) => ViewBag.AsientosJson = json ?? ListaJson(modelo.Asientos);

        public IActionResult Index(string? buscar)
        {
            var datos = _context.Aeronaves.Find(x => true).ToList();
            if (!string.IsNullOrWhiteSpace(buscar)) datos = datos.Where(x => x.Matricula.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Modelo.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Modelo.Fabricante.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList();
            ViewBag.Buscar = buscar; return View(datos);
        }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var dato = _context.Aeronaves.Find(x => x.Id == oid).FirstOrDefault(); return dato == null ? NotFound() : View(dato); }
        [HttpGet] public IActionResult Create() { var modelo = new Aeronave(); Preparar(modelo, "[]"); return View(modelo); }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Aeronave modelo, string? asientosJson)
        {
            try { modelo.Asientos = LeerLista<AsientoAeronave>(asientosJson); }
            catch { ModelState.AddModelError("asientosJson", "El JSON de asientos no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, asientosJson); return View(modelo); }
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Aeronaves.InsertOne(modelo); TempData["Mensaje"] = "Aeronave creada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar. Revise la matrícula y el ID SQL, que deben ser únicos."); Preparar(modelo, asientosJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var dato = _context.Aeronaves.Find(x => x.Id == oid).FirstOrDefault(); if (dato == null) return NotFound(); Preparar(dato); return View(dato); }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Aeronave modelo, string? asientosJson)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound();
            try { modelo.Asientos = LeerLista<AsientoAeronave>(asientosJson); } catch { ModelState.AddModelError("asientosJson", "El JSON de asientos no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, asientosJson); return View(modelo); }
            try { modelo.Id = oid; var r = _context.Aeronaves.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Aeronave actualizada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar. Revise la matrícula y el ID SQL."); Preparar(modelo, asientosJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var dato = _context.Aeronaves.Find(x => x.Id == oid).FirstOrDefault(); return dato == null ? NotFound() : View(dato); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Aeronaves.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Aeronave eliminada correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
