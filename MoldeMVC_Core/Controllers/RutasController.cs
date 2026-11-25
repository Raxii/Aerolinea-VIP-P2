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
    public class RutasController : Controller
    {
        private readonly MongoDbContext _context;
        public RutasController(MongoDbContext context) { _context = context; }
        private static string ListaJson<T>(IEnumerable<T> lista) { var a = new BsonArray(); foreach (var x in lista) a.Add(x.ToBsonDocument()); return a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }); }
        private static List<T> LeerLista<T>(string? json) => string.IsNullOrWhiteSpace(json) ? new List<T>() : BsonSerializer.Deserialize<List<T>>(json);
        private void Preparar(Ruta m, string? json = null) => ViewBag.FrecuenciasJson = json ?? ListaJson(m.Frecuencias);
        private bool AsignarIds(Ruta m, string? origenAeropuertoId, string? destinoAeropuertoId)
        {
            bool ok = true;
            if (!ObjectId.TryParse(origenAeropuertoId, out var origen)) { ModelState.AddModelError("origenAeropuertoId", "Ingrese un ObjectId válido para el aeropuerto de origen."); ok = false; } else m.Origen.AeropuertoId = origen;
            if (!ObjectId.TryParse(destinoAeropuertoId, out var destino)) { ModelState.AddModelError("destinoAeropuertoId", "Ingrese un ObjectId válido para el aeropuerto de destino."); ok = false; } else m.Destino.AeropuertoId = destino;
            return ok;
        }
        public IActionResult Index(string? buscar) { var d = _context.Rutas.Find(x => true).ToList(); if (!string.IsNullOrWhiteSpace(buscar)) d = d.Where(x => x.Origen.CodigoIATA.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Destino.CodigoIATA.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Origen.Localidad.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Destino.Localidad.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar)).ToList(); ViewBag.Buscar = buscar; return View(d); }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Rutas.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpGet] public IActionResult Create() { var m = new Ruta { Activa = true }; Preparar(m, "[]"); return View(m); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(Ruta modelo, string? origenAeropuertoId, string? destinoAeropuertoId, string? frecuenciasJson)
        {
            AsignarIds(modelo, origenAeropuertoId, destinoAeropuertoId);
            try { modelo.Frecuencias = LeerLista<FrecuenciaRuta>(frecuenciasJson); } catch { ModelState.AddModelError("frecuenciasJson", "El JSON de frecuencias no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, frecuenciasJson); return View(modelo); }
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Rutas.InsertOne(modelo); TempData["Mensaje"] = "Ruta creada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar la ruta. Revise el ID SQL y los datos embebidos."); Preparar(modelo, frecuenciasJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Rutas.Find(x => x.Id == oid).FirstOrDefault(); if (d == null) return NotFound(); Preparar(d); ViewBag.OrigenId = d.Origen.AeropuertoId.ToString(); ViewBag.DestinoId = d.Destino.AeropuertoId.ToString(); return View(d); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Edit(string id, Ruta modelo, string? origenAeropuertoId, string? destinoAeropuertoId, string? frecuenciasJson)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound(); AsignarIds(modelo, origenAeropuertoId, destinoAeropuertoId);
            try { modelo.Frecuencias = LeerLista<FrecuenciaRuta>(frecuenciasJson); } catch { ModelState.AddModelError("frecuenciasJson", "El JSON de frecuencias no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, frecuenciasJson); ViewBag.OrigenId = origenAeropuertoId; ViewBag.DestinoId = destinoAeropuertoId; return View(modelo); }
            try { modelo.Id = oid; var r = _context.Rutas.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Ruta actualizada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar la ruta."); Preparar(modelo, frecuenciasJson); ViewBag.OrigenId = origenAeropuertoId; ViewBag.DestinoId = destinoAeropuertoId; return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Rutas.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Rutas.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Ruta eliminada correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
