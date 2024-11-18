namespace PIGGISWS.Models;

public class DDocumento
{
    public int DDO_EMPRESA {  get; set; }
    public decimal DDO_CCO_COMPROBA { get; set; }
    public decimal DDO_TRANSACC { get; set; }

    public string DDO_DOCTRAN { get; set; } = string.Empty;

    public decimal DDO_PAGO { get; set; }
    public decimal DDO_CODCLIPRO { get; set; }
    public int DDO_DEBCRE {  get; set; }
    public DateTime DDO_FECHA_EMI { get; set; }
    public DateTime DDO_FECHA_VEN { get; set; }

    public decimal DDO_MONTO { get; set; }
    public decimal DDO_MONTO_EXT { get; set; }
    public decimal DDO_RECARGOS { get; set; }
    public decimal DDO_RECARGOS_EXT { get; set; }
    public decimal? DDO_CANCELA { get; set; }

    public decimal DDO_CANCELA_EXT { get; set; }
    public int DDO_CANCELADO { get; set; }
    public decimal DDO_AGENTE { get; set; }

}
