namespace PIGGISWS.Models.DTOs;

public class AuxPresupuesto
{
    public decimal? AGENTE { get; set; }
    public int? MES { get; set; } 
    public int? PERIODO { get; set; }
    public int? DIA { get; set; }
    public int? DIAF { get; set; }
    public List<PresupuestoDetalle> Detalles { get; set; } = new(); 
    public decimal TotalFacturado { get; set; } 
    public decimal TotalPresupuesto { get; set; }
    public decimal TotalCumplimiento { get; set; } 
}

public class PresupuestoDetalle
{
    public string Tipo { get; set; } // "Baja", "Alta" o "Media"
    public decimal Kilos { get; set; } 
    public decimal Presupuesto { get; set; } 
    public decimal Cumplimiento { get; set; } 
}
