using Microsoft.Graph;

namespace PIGGISWS.Models;

public class CComprobai
{

    public int CCO_EMPRESA { get; set; }
    public int CCO_CODIGO { get; set; }
    public int CCO_PERIODO { get; set; }
    public int CCO_SIGLA {  get; set; }
    public int? CCO_ALMACEN { get; set; }
    public int CCO_SERIE { get; set; }
    public string? CCO_DOCTRAN { get; set; } = string.Empty;
    public int CCO_TIPODOC { get; set; }
    public DateTime? CCO_FECHA { get; set; }
    public string CCO_CONCEPTO { get; set; } = string.Empty;
    public int CCO_MODULO { get; set; }
    public int CCO_NOCONTABLE { get; set; }
    public int CCO_ESTADO { get; set; }
    public int CCO_DESCUADRE { get; set; }
    public int CCO_ADESTINO { get; set; }
    public int? CCO_PVENTA { get; set; }
    public int? CCO_CENTRO {  get; set; } 

    public decimal CCO_TIPO_CAMBIO { get; set; }
    public int? CCO_TCLIPRO { get; set; }
    public int? CCO_CODCLIPRO { get; set; }
    public int? CCO_AGENTE { get; set; }
    public int? CCO_TRANSACC {  get; set; }
    public int CCO_ANULADO { get; set; }
    public int CCO_BODEGA { get; set; }
    public int? CCO_DIA {  get; set; }
    public int? CCO_MES { get; set; }
    public int? CCO_ANIO { get; set; }
    public string? CCO_DETALLE { get; set; } = string.Empty;
}
