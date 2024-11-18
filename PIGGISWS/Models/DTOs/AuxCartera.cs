namespace PIGGISWS.Models.DTOs;

public class AuxCartera
{
    public DDocumento? dDocumento { get; set; }
    public Cartera? cartera { get; set; }
    public string? AGE_NOMBRE { get; set; } = string.Empty;
    public string? CLI_NOMBRE { get; set; } = string.Empty;
    public string? ZON_NOMBRE { get; set; } = string.Empty;
    public decimal? SALDO { get; set; }
    public int? CRT_SECUENCIA { get; set; }
    public decimal? CRT_MONTO { get; set; }
    public decimal? CRT_CANCELA { get; set; }
    public decimal? CRT_CANCELA_CH {  get; set; }
    public int? CRT_NUMERO { get; set; }
    public string? DDO_DOCTRAN { get; set; } = string.Empty;
    public DateTime? CRT_FECHA { get; set; }
}
