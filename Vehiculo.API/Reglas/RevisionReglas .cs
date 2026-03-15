using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos.Servicios.Revision;
using System;

namespace Reglas
{
    public class RevisionReglas : IRevisionReglas
    {
        private readonly IRevisionServicio _RevisionServicio;
        private readonly IConfiguracion _configuracion;

        public RevisionReglas(IRevisionServicio RevisionServicio, IConfiguracion configuracion)
        {
            _RevisionServicio = RevisionServicio;
            _configuracion = configuracion;
        }

        public async Task<bool> RevisionEsValida(string placa)
        {
            var resultadoRevision = await _RevisionServicio.Obtener(placa);

            if (resultadoRevision == null)
                return false;

            if (!ValidarEstado(resultadoRevision))
                return false;

            if (resultadoRevision.Periodo == null)
                return false;

            if (!ValidarPeriodo(resultadoRevision.Periodo))
                return false;

            return true;
        }

        private bool ValidarEstado(Revision resultadoRevision)
        {
            if (resultadoRevision == null)
                return false;

            if (resultadoRevision.Resultado == null)
                return false;

            string estadoRevision = _configuracion.ObtenerValor("EstadoRevisionSastifactoria");

            if (estadoRevision == null)
                return false;

            return resultadoRevision.Resultado == estadoRevision;
        }

        private static string ObtenerPeriodoActual()
        {
            return $"{DateTime.Now.Month}-{DateTime.Now.Year}";
        }

        private static bool ValidarPeriodo(string periodo)
        {
            if (periodo == null)
                return false;

            var periodoActual = ObtenerPeriodoActual();
            return periodo == periodoActual;
        }
    }
}