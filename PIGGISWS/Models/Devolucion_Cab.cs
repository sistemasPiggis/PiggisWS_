using Microsoft.Graph;

namespace PIGGISWS.Models;

public class Devolucion_Cab
{
    public decimal DEV_CODIGO { get; set; }
    public decimal? DEV_NUMERO { get; set; }
    public decimal? DEV_CLIENTE { get; set; }
    public decimal? DEV_AGENTE { get; set; }
    public DateTime? DEV_FECHA { get; set; }
    public string? DEV_OBSERVACION { get; set; } = string.Empty;
    public decimal? DEV_NUM_FUNDAS { get; set; } 
    public int? DEV_IMPRESO { get; set; }
    public decimal? DEV_DOC_REFERENCIA { get; set; }
    public string? DEV_ESTADO { get; set; } = string.Empty;
    public string? DEV_OBSERVACION_VALIDADOR { get; set; } = string.Empty;
}
