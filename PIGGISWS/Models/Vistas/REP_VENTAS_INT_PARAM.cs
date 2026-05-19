namespace PIGGISWS.Models.Vistas;

public class REP_VENTAS_INT_PARAM
{
    public decimal CCO_CODCLIPRO { get; set; }
    public decimal PRO_CODIGO { get; set; }
    public string PRO_ID { get; set; } = string.Empty;
    public string PRO_NOMBRE { get; set; } = string.Empty;
    public string UMD_CODIGO { get; set; } = string.Empty;
    public string UMD_ID { get; set; } = string.Empty;

    public decimal FAC_CANTIDAD_ORIGINAL { get; set; }
    public decimal? FAC_CANTIDAD_ORIGINAL2 { get; set; }
    public decimal PRO_CANTIDAD { get; set; }
    public decimal? EXTEMPORANEO { get; set; }
    public int? DVD_SECUENCIA { get; set; }
    public int? DFAC_SECUENCIA {get; set; }
}
