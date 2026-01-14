using System.Security.Policy;

namespace PIGGISWS.Models;

public class CCOMFACI_EXT
{
    public int? CFAI_EMPRESA { get; set; }
    public decimal? CFAI_CCO_COMPROBA { get; set; }
    public string? CREA_USR { get; set; } = string.Empty;
    public DateTime? CREA_FECHA { get; set; }
    public string? MOD_USR { get; set; } = string.Empty;
    public DateTime? MOD_FECHA { get; set; }
    public int? CFAI_INACTIVO { get; set; }
    public string? CFAI_REFERENCIA_UNICA_TX { get; set; } = string.Empty;
    public int? CFAI_SECUENCIAL_MOVIL { get; set; }
    public double? CFAI_LATITUD_NR { get; set; } 
    public double? CFAI_LONGITUD_NR { get; set; }
    public DateTime? CFAI_FECHA_CREACION_ORG_DT { get; set; }

}
