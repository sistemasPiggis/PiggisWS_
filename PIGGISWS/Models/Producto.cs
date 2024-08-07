namespace PIGGISWS.Models;

public class Producto
{
    public int PRO_EMPRESA {  get; set; }
    public int PRO_CODIGO { get; set; }
    public string PRO_ID { get; set; } = string.Empty;
    public string PRO_NOMBRE { get; set; } = string.Empty;
    public int? PRO_INACTIVO { get; set; }
    public int? PRO_UNIDAD { get; set; }
    public int? PRO_UNIDAD2 { get; set; }
    public int? PRO_IMPUESTO { get; set; }   
    public int? PRO_PROMOCION {  get; set; }
    public int? PRO_CRITICO { get; set; }
}
