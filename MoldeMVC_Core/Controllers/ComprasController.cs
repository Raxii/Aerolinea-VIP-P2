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
    public class ComprasController : Controller
    {
        private readonly MongoDbContext _context;
        public ComprasController(MongoDbContext context) { _context = context; }
        private static string ListaJson<T>(IEnumerable<T> lista) { var a = new BsonArray(); foreach (var x in lista) a.Add(x.ToBsonDocument()); return a.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { Indent = true }); }
        private static List<T> LeerLista<T>(string? json) => string.IsNullOrWhiteSpace(json) ? new List<T>() : BsonSerializer.Deserialize<List<T>>(json);
        private void Preparar(Compra m, string? pagos = null, string? comprobantes = null) { ViewBag.PagosJson = pagos ?? ListaJson(m.Pagos); ViewBag.ComprobantesJson = comprobantes ?? ListaJson(m.Comprobantes); }
        public IActionResult Index(string? buscar) { var d = _context.Compras.Find(x => true).ToList(); if (!string.IsNullOrWhiteSpace(buscar)) d = d.Where(x => x.Estado.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.Moneda.Contains(buscar, StringComparison.OrdinalIgnoreCase) || x.IdSql.ToString().Contains(buscar) || x.ClaveIdempotencia.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList(); ViewBag.Buscar = buscar; return View(d); }
        public IActionResult Details(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Compras.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpGet] public IActionResult Create() { var m = new Compra { FechaCompra = DateTime.Now, Moneda = "USD", Estado = "PENDIENTE" }; Preparar(m, "[]", "[]"); return View(m); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(Compra modelo, string? reservaId, string? pagosJson, string? comprobantesJson)
        {
            if (!ObjectId.TryParse(reservaId, out var reserva)) ModelState.AddModelError("reservaId", "Ingrese un ObjectId válido para la reserva."); else modelo.ReservaId = reserva;
            try { modelo.Pagos = LeerLista<PagoCompra>(pagosJson); } catch { ModelState.AddModelError("pagosJson", "El JSON de pagos no es válido."); }
            try { modelo.Comprobantes = LeerLista<ComprobanteCompra>(comprobantesJson); } catch { ModelState.AddModelError("comprobantesJson", "El JSON de comprobantes no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, pagosJson, comprobantesJson); return View(modelo); }
            try { modelo.Id = ObjectId.GenerateNewId(); _context.Compras.InsertOne(modelo); TempData["Mensaje"] = "Compra creada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo guardar la compra. Revise el ID SQL y la estructura de pagos/comprobantes."); Preparar(modelo, pagosJson, comprobantesJson); return View(modelo); }
        }
        [HttpGet] public IActionResult Edit(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Compras.Find(x => x.Id == oid).FirstOrDefault(); if (d == null) return NotFound(); Preparar(d); ViewBag.ReservaId = d.ReservaId.ToString(); return View(d); }
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Edit(string id, Compra modelo, string? reservaId, string? pagosJson, string? comprobantesJson)
        {
            if (!ObjectId.TryParse(id, out var oid)) return NotFound(); if (!ObjectId.TryParse(reservaId, out var reserva)) ModelState.AddModelError("reservaId", "Ingrese un ObjectId válido para la reserva."); else modelo.ReservaId = reserva;
            try { modelo.Pagos = LeerLista<PagoCompra>(pagosJson); } catch { ModelState.AddModelError("pagosJson", "El JSON de pagos no es válido."); }
            try { modelo.Comprobantes = LeerLista<ComprobanteCompra>(comprobantesJson); } catch { ModelState.AddModelError("comprobantesJson", "El JSON de comprobantes no es válido."); }
            if (!ModelState.IsValid) { Preparar(modelo, pagosJson, comprobantesJson); ViewBag.ReservaId = reservaId; return View(modelo); }
            try { modelo.Id = oid; var r = _context.Compras.ReplaceOne(x => x.Id == oid, modelo); if (r.MatchedCount == 0) return NotFound(); TempData["Mensaje"] = "Compra actualizada correctamente."; return RedirectToAction(nameof(Index)); }
            catch { ModelState.AddModelError(string.Empty, "No se pudo actualizar la compra."); Preparar(modelo, pagosJson, comprobantesJson); ViewBag.ReservaId = reservaId; return View(modelo); }
        }
        [HttpGet] public IActionResult Delete(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); var d = _context.Compras.Find(x => x.Id == oid).FirstOrDefault(); return d == null ? NotFound() : View(d); }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken] public IActionResult DeleteConfirmed(string id) { if (!ObjectId.TryParse(id, out var oid)) return NotFound(); _context.Compras.DeleteOne(x => x.Id == oid); TempData["Mensaje"] = "Compra eliminada correctamente."; return RedirectToAction(nameof(Index)); }
    }
}
