
using MongoDB.Bson.Serialization.Conventions;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Data
{
    public static class MongoConvenciones
    {
        public static void Registrar()
        {
            var convenciones = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(
                "AerolineaVIP_Modelos",
                convenciones,
                tipo => tipo.Namespace == typeof(Vuelo).Namespace);
        }
    }
}
