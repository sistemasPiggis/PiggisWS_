namespace PIGGISWS.Models;

public class Cmovinv
{
    public int CMO_EMPRESA {  get; set; }
    public decimal CMO_CCO_COMPROBA { get; set; }
    public decimal CMO_TRANSACC { get; set; }
    public decimal? CMO_CCO_PEDIDO { get; set; }
    public decimal? CMO_CCO_FACTURA { get; set; }
    public decimal? CMO_CCO_REFERENCIA { get; set; }
    public string? CMO_REFERENCIA { get; set; } = string.Empty;
    public decimal? CMO_CLIENTE {  get; set; }
}
