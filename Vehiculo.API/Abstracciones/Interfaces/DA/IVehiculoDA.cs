using Abstracciones.Modelos;


namespace Abstracciones.Interfaces.DA
{
    public interface IVehiculoDA
    {
       Task<IEnumerable<VehiculoResponse>> Obtener();
       Task<VehiculoDetalle> Obtener(Guid id);
       Task<Guid> Agregar(VehiculoRequest vehiculo);
       Task<Guid> Editar(Guid ID, VehiculoRequest vehiculo);
       Task<Guid> Eliminar(Guid id);
    }
}
