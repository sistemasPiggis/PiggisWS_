using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PIGGISWS.Models;

public class Plantilla_Revision
{
    [Key]
    [Display(Name = "Plantilla Codigo")]
    public int PLN_CODIGO {  get; set; }
    [Display(Name = "Plantilla Emoresa")]
    public int PLN_EMPRESA {   get; set; }
    [Display(Name = "Plantilla ID")]
    public string PLN_ID { get; set;} = string.Empty;
    [Display(Name = "Plantilla Nombre")]
    public string PLN_NOMBRE { get; set; } = string.Empty;
    [Display(Name = "Plantilla Descripción")]
    public string PLN_DESCRIPCION { get; set; } = string.Empty;
    [Display(Name = "Plantilla Estado")]
    public int PLN_ESTADO { get; set; }
    [Display(Name = "Plantilla Especificación")]
    public string PLN_ESPECIFICACION {  get; set; } = string.Empty;
    [Display(Name = "Plantilla Acción Correctiva")]
    public string PLN_ACCION_CORRECTIVA { get; set; } = string.Empty;
    [Display(Name = "Plantilla Referencia")]
    public string PLN_REFERENCIA { get; set; } = string.Empty;
    [Display(Name = "Plantilla Nota")]
    public string PLN_NOTA { get; set; } = string.Empty;
}
