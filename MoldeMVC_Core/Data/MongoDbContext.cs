using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDb:ConnectionString"]!);
            _database = client.GetDatabase(configuration["MongoDb:DatabaseName"]!);
        }

        public IMongoCollection<Catalogo> Catalogos =>
            _database.GetCollection<Catalogo>("catalogos");

        public IMongoCollection<Aeropuerto> Aeropuertos =>
            _database.GetCollection<Aeropuerto>("aeropuertos");

        public IMongoCollection<Ruta> Rutas =>
            _database.GetCollection<Ruta>("rutas");

        public IMongoCollection<Aeronave> Aeronaves =>
            _database.GetCollection<Aeronave>("aeronaves");

        public IMongoCollection<Vuelo> Vuelos =>
            _database.GetCollection<Vuelo>("vuelos");

        public IMongoCollection<Pasajero> Pasajeros =>
            _database.GetCollection<Pasajero>("pasajeros");

        public IMongoCollection<Itinerario> Itinerarios =>
            _database.GetCollection<Itinerario>("itinerarios");

        public IMongoCollection<Reserva> Reservas =>
            _database.GetCollection<Reserva>("reservas");

        public IMongoCollection<Asignacion> Asignaciones =>
            _database.GetCollection<Asignacion>("asignaciones");

        public IMongoCollection<Compra> Compras =>
            _database.GetCollection<Compra>("compras");

        public IMongoCollection<DatosEmisor> DatosEmisor =>
            _database.GetCollection<DatosEmisor>("datosEmisor");
    }
}
