using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PIGGISWS.Models;

public class Plantilla_Revision
{
    [Key]
    public int PLN_CODIGO {  get; set; }
    public int PLN_EMPRESA {   get; set; }
    public string PLN_ID { get; set;} = string.Empty;
    public string PLN_NOMBRE { get; set; } = string.Empty;
    public string PLN_DESCRIPCION { get; set; } = string.Empty;
    public int PLN_ESTADO { get; set; } 
    public string PLN_ESPECIFICACION {  get; set; } = string.Empty;
    public string PLN_ACCION_CORRECTIVA { get; set; } = string.Empty;
    public string PLN_REFERENCIA { get; set; } = string.Empty;
    public string PLN_NOTA { get; set; } = string.Empty;
}
