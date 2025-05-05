namespace PIGGISWS.Models.DTOs;

public class DetalleCanal
{
    public string Canal { get; set; } // Nombre del canal (TES_NOMBRE)
    public decimal Kilos { get; set; } // Total de kilos
    public decimal Monto { get; set; } // Total del monto
    public int Clientes { get; set; } // Número de clientes únicos
}
