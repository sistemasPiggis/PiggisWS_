namespace PIGGISWS.Models.Vistas;

public class REP_DET_ESTADO_DESPACHO_PED
{
    public DateTime CRT_FECHA { get; set; } = DateTime.Now;
    public string? EMP_NOMBRE { get; set; } = string.Empty;
    public string? ZON_NOMBRE { get; set; } = string.Empty;
    public string? AGE_NOMBRE { get; set; } = string.Empty;
    public string? CLI_NOMBRE { get; set; } = string.Empty;
    public string? FAC { get; set; } = string.Empty;
    public string? PEC { get; set; } = string.Empty;
    public string? ESTADO_DESPACHO { get; set; } = string.Empty;
    public decimal? CFAC_CCO_PEDIDO { get; set; }
    public decimal? AGE_CODIGO { get; set; }
}
