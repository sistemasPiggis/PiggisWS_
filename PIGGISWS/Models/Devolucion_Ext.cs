namespace PIGGISWS.Models;

public class Devolucion_Ext
{
    public int DEV_EMPRESA { get; set; }
    public decimal? DEV_CODIGO { get; set; }
    public string? DEV_REFERENCIA_UNICA_TX { get; set; } = string.Empty;
    public decimal? DEV_SECUENCIAL_MOVIL { get; set; }
    public double? DEV_LATITUD_NR { get; set; }
    public double? DEV_LONGITUD_NR { get; set; }
    public DateTime? DEV_FECHA_CREACION_ORG_DT { get; set; }

}
