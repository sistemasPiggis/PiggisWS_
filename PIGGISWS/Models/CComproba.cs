using System.Numerics;

namespace PIGGISWS.Models;

public class CComproba
{
    public int CCO_EMPRESA {  get; set; }

    public decimal CCO_CODIGO { get; set;}
    public DateTime CCO_FECHA { get; set;}
    public int CCO_SIGLA {  get; set;}
    public decimal CCO_CODCLIPRO { get; set;}
    public int CCO_AGENTE { get; set;}

    public int CCO_ESTADO { get; set;}
    public int CCO_NUMERO {  get; set;}
    public string? CCO_DETALLE {  get; set;} = string.Empty;
    public int CCO_DIA {  get; set;}
    public int CCO_MES { get; set; }
    public int CCO_PERIODO { get; set;}
    public decimal? CCO_CIE_COMPROBA { get; set;}
    public int? CCO_ALMACEN { get; set; }

}
