namespace PIGGISWS.Models.DTOs;

public class AuxDevolucion
{
    public Devolucion_Cab? Cabecera { get; set; }
    public Devolucion_Ext? Ext { get; set; }
    public List<Devolucion_Det>? Detalle { get; set; }
}
