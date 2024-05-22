using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Planifica_Revision
{

    public int? PLR_EMPRESA {  get; set; }
    [Key]
    [Display(Name = "Codigo Planificación")]
    public int PLR_CODIGO { get; set; }
    [Display(Name = "Planificación Número")]
    public int? PLR_NUMERO { get; set; }
    [Display(Name = "Planificación Estado")]
    public int? PLR_ESTADO { get; set; }
    [Display(Name = "Planificación Descripción")]
    public string? PLR_DESCRIPCION { get; set; } = string.Empty;

}
