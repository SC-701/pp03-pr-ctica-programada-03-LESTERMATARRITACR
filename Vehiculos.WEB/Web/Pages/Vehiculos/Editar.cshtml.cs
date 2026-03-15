using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    public class EditarModel : PageModel
    {
        private IConfiguracion _configuracion;

        public EditarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        public VehiculoResponse vehiculo { get; set; } = default!;

        public VehiculoRequest vehiculoRequest { get; set; } = default!;

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new();

        [BindProperty]
        public Guid marcaSeleccionada { get; set; }

        [BindProperty]
        public Guid modeloSeleccionado { get; set; }

        public async Task<ActionResult> OnGet(Guid? id)
        {
            if (id == null)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");
            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);
            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                await ObtenerMarcasAsync();

                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                vehiculo = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones);

                if (vehiculo != null)
                {
                    marcaSeleccionada = Guid.Parse(
                        marcas.First(m => m.Text == vehiculo.Marca).Value
                    );

                    modelos = (await ObtenerModelosAsync(marcaSeleccionada))
                        .Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Nombre,
                            Selected = a.Nombre == vehiculo.Modelo
                        }).ToList();

                    var modelo = modelos.FirstOrDefault(m => m.Text == vehiculo.Modelo);

                    if (modelo != null)
                        modeloSeleccionado = Guid.Parse(modelo.Value);
                }
            }

            return Page();
        }

        public async Task<ActionResult> OnPost()
        {
            if (vehiculo.Id == Guid.Empty)
                return NotFound();


            if (!ModelState.IsValid)
            {
                await ObtenerMarcasAsync();

                if (marcaSeleccionada != Guid.Empty)
                {
                    modelos = (await ObtenerModelosAsync(marcaSeleccionada))
                        .Select(a => new SelectListItem
                        {
                            Value = a.Id.ToString(),
                            Text = a.Nombre,
                            Selected = a.Id == modeloSeleccionado
                        }).ToList();
                }

                return Page();
            }
       
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EditarVehiculo");

            var cliente = new HttpClient();

            var respuesta = await cliente.PutAsJsonAsync(
                string.Format(endpoint, vehiculo.Id.ToString()),
                new VehiculoRequest
                {
                    IdModelo = modeloSeleccionado,
                    Anio = vehiculo.Anio,
                    Color = vehiculo.Color,
                    CorreoPropietario = vehiculo.CorreoPropietario,
                    Placa = vehiculo.Placa,
                    Precio = vehiculo.Precio,
                    TelefonoPropietario = vehiculo.TelefonoPropietario
                });

            respuesta.EnsureSuccessStatusCode();

            return RedirectToPage("./Index");
        }

        private async Task ObtenerMarcasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");

            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var respuesta = await cliente.SendAsync(solicitud);

            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();

                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, opciones);

                marcas = resultadoDeserializado.Select(a =>
                    new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = a.Nombre
                    }).ToList();
            }
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var modelos = await ObtenerModelosAsync(marcaId);
            return new JsonResult(modelos);
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");

            var cliente = new HttpClient();

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, marcaId));

            var respuesta = await cliente.SendAsync(solicitud);

            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();

                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<Modelo>>(resultado, opciones);
            }

            return new List<Modelo>();
        }
    }
}