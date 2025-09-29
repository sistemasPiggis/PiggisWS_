namespace PIGGISWS.Models.DTOs;

public class PedidoNavCab
{
    public int ID_PEDIDO_NAV { get; set; }
    public decimal CLI_CODIGO { get; set; }
    public string CLI_NOMBRE { get; set; } = string.Empty;
    public DateTime FECHA { get; set; }
    public string TELEFONO { get; set; } = string.Empty;
    public string OBSERVACIONES { get; set; } = string.Empty;

}
