namespace PIGGISWS.Models;

public class TDS_PEDIDOS_NAV_CAB
{

    public int ID_PEDIDO_NAV { get; set; }
    public decimal CLI_CODIGO { get; set; } 

    public DateTime FECHA { get; set; } 
    public string TELEFONO { get; set; } = string.Empty;
    public string OBSERVACIONES { get; set; } = string.Empty;

    public string? CREA_USR { get; set; } = string.Empty;
    public DateTime? CCO_FECHA { get; set; }
    public string? MOD_USR { get; set; }
    public DateTime? MOD_FECHA { get; set; } = DateTime.Now;
}
