namespace PIGGISWS.Models;

public class TDS_PEDIDO_NAV_DET
{

    public int ID_PEDIDO_DET { get; set; }
    public int ID_EMPRESA { get; set; }
    public int ID_PEDIDO_FK { get; set; } = 1;
    public decimal PRO_CODIGO { get; set; }
    public decimal CANTIDAD { get; set; }
    public decimal PRECIO { get; set; }
    public decimal UMD_CODIGO { get; set; }
}