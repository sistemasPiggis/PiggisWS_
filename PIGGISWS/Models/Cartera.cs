namespace PIGGISWS.Models;

public class Cartera
{
    public int? CRT_EMPRESA { get; set; }
    public decimal? CRT_DDO_COMPROBA { get; set; }
    public decimal? CRT_TRANSACC { get; set; }
    public string? CRT_DOCTRAN { get; set; } = string.Empty;
    private decimal? _crtPago = 1; // Valor por defecto
    public decimal? CRT_PAGO
    {
        get => _crtPago;
        set => _crtPago = value ?? 1; // Si el valor es null, asigna 1
    }
    public DateTime? CRT_FECHA { get; set; }
    public decimal? CRT_CLIENTE { get; set; }
    public int? CRT_AGENTE { get; set; }
    public decimal? CRT_MONTO { get; set; } // Monto a cobrar = monto de documento menos lo cancelado
    //public int CRT_EMPLEADO { get; set; }
    public decimal? CRT_NUMERO { get; set; }
    public int? CRT_PROCESADA { get; set; }
    public decimal? CRT_CANCELA { get; set; }
    public decimal? CRT_CANCELA_CH { get; set; }
    public int? CRT_SECUENCIA { get; set; }
    public int? CRT_ESTADO { get; set; }
    public decimal? CRT_EMPLEADO { get; set; }
    public string? CREA_USR { get; set; }

}
