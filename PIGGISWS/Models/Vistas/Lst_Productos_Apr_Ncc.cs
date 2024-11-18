namespace PIGGISWS.Models.Vistas;

public class Lst_Productos_Apr_Ncc
{

    public int CCO_EMPRESA {  get; set; }
    public decimal CCO_CODIGO { get; set; }
    public string ID { get; set; } = string.Empty;

    public string PRODUCTO { get; set; } = string.Empty;

    public string UNIDAD {  get; set; } = string.Empty;
    public decimal CANTIDAD { get; set; }
    public decimal VALOR { get; set; }
    public string FACTURA {  get; set; } = string.Empty;
    public string MOTIVO { get; set; } = string.Empty;

}
