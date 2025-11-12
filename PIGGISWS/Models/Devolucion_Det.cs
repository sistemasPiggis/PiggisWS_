namespace PIGGISWS.Models;

public class Devolucion_Det
{
    public decimal DVD_CODIGO { get; set; }
    public decimal? DVD_PRODUCTO { get; set; }
    public decimal? DVD_UNIDAD { get; set; }
    public decimal? DVD_CANTIDAD { get; set; }
    public decimal? DVD_REFERENCIA { get; set; }
    public decimal? DVD_FACTURA { get; set; }

    public int DVD_PROCESA { get; set; } = 1; // 
    public string? DVD_OBSERVACION { get; set; } = string.Empty;

    public decimal? DVD_MOTIVO { get; set; }
    public int DVD_SECUENCIA { get; set; }
    public string DVD_CARGADO_DESDE { get; set; } = "V"; // V= AGENTE, A = ADMINISTRACION

    public string DVD_ESTADO { get; set; } = "G"; // A= ACTIVO, I = INACTIVO
    public string? DVD_OBSERVACION_CALIDAD2 { get; set; } = string.Empty;
}
