namespace PIGGISWS.Models;

public class DListapre
{
    public int DLP_EMPRESA {  get; set; }
    public int DLP_LISTAPRE { get; set; }
    public int DLP_CODIGO { get; set; }
    public int DLP_ALMACEN { get; set; }
    public int DLP_TPRODUCTO { get; set; }
    public int DLP_PRODUCTO { get; set; }
    public int DLP_CATPRODUCTO { get; set; }
    public decimal DLP_PRECIO { get; set; }
    public int? DLP_INACTIVO { get; set; }
    public DateTime DLP_FECHA_INI { get; set; }
    public DateTime? DLP_FECHA_FIN {  get; set; }
    public DateTime? CREA_FECHA { get; set; }
    public decimal DLP_DESCUENTO { get; set; }
    public decimal DLP_PRECIO2 { get; set; }
}
