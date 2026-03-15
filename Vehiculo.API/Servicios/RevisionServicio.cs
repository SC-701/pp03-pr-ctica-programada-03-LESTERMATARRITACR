using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Servicios.Revision;
using System.Text.Json;

namespace Servicios
{
    public class RevisionServicio : IRevisionServicio
    {
        private readonly IConfiguracion _configuracion;
        private readonly IHttpClientFactory _httpClient;

        public RevisionServicio(IConfiguracion configuracion, IHttpClientFactory httpClient)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
        }

        public async Task<Revision?> Obtener(string placa)
        {
            // ✅ NOMBRE CORRECTO
            var endPoint = _configuracion.ObtenerMetodo("ApiEndPointsRevision", "ObtenerRevision");

            var servicioRegistro = _httpClient.CreateClient("ServicioRevision");
            var respuesta = await servicioRegistro.GetAsync(string.Format(endPoint, placa));

            if (!respuesta.IsSuccessStatusCode)
                return null;

            var resultado = await respuesta.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var lista = JsonSerializer.Deserialize<List<Revision>>(resultado, opciones);
            return lista?.FirstOrDefault();
        }
    }
}