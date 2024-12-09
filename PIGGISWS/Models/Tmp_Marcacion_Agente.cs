using Microsoft.Graph;

namespace PIGGISWS.Models;

public class Tmp_Marcacion_Agente
{
    public decimal ID_MARCACION { get; set; }
    public int ID_EMPRESA { get; set; }
    public decimal AGE_CODIGO { get; set; }
    public DateTime MAR_FECHA { get; set; }
    public string? ENTRADA1_MOVIL { get; set; } = string.Empty;

    public string? ENTRADA1 { get; set; } = string.Empty;
    public string? SALIDA1_MOVIL { get; set; }  = string.Empty;
    public string? SALIDA1 { get; set; } = string.Empty;
    public string? ENTRADA2_MOVIL { get; set; } = string.Empty;
    public string? ENTRADA2 { get; set; } = string.Empty;
    public string? SALIDA2_MOVIL { get; set; } = string.Empty;
    public string? SALIDA2 { get; set; } = string.Empty;
    public string? UBICACION1 { get; set; } = string.Empty;
    public string? UBICACION2 { get; set; } = string.Empty;
    public string? UBICACION3 { get; set; } = string.Empty;
    public string? UBICACION4 { get; set; } = string.Empty;

    public DateTime? MOD_FECHA { get; set; }
}
