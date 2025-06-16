namespace PIGGISWS.Models;

public class Tmp_Dev_Pro_Cli_Age
{

   public decimal CODIGO_PK
    {
        get;
        set;
    }
    public decimal CLIENTE_FK { get; set; }
    public decimal AGENTE_FK { get; set; }
    public decimal PRODUCTO_FK { get; set; }
    public decimal? CANTIDAD_NR { get; set; }
    public decimal? MOTIVO_FK { get; set; }
    public string ESTADO_TX { get; set; } = string.Empty;
    public DateTime? FECHA_DT { get; set; }
    public decimal ID_DEVOLUCION_NR { get; set; }
    public decimal? NUMERO_IDC { get; set; }
    public string? OBSERVACION_TX { get; set; } = string.Empty;
    public decimal? UNIDAD_INGRESO_NR { get; set; }
    public decimal? CANTIDAD_INGRESO_NR { get; set; }
    public decimal? REFERENCIA_NR { get; set; }
    public string? FACTURA_TX { get; set; } = string.Empty;
    public int? NUMERO_FUNDAS { get; set; }
    public int? IMPRESO { get; set; }
    public string? OBSERVACION { get; set; } = string.Empty;
    public int? PROCESA { get; set; }
    public DateTime? FECHA_IMPRESION { get; set; }   
    public decimal? USR_IMPRIME { get; set; }
}
