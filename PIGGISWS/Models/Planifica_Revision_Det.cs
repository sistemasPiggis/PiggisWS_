using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Planifica_Revision_Det
{
    [Key]
    public int PLD_CODIGO { get; set; }
    public int? PLR_CODIGO { get; set;}
    public int? PLN_CODIGO { get; set; }
    public DateTime? PLD_FECHA { get; set; } = DateTime.Now;

    public string? PLD_HORA { get; set; } = string.Empty;

    [Display(Name = "Empleado que Solicita")]
    public int? PLD_EMP_SOLICITA { get; set; }
    [Display(Name = "Empleado que Finaliza")]
    public int? PLD_EMP_FINALIZA { get; set; }
    [Display(Name = "Observaciones")]
    public string? PLD_OBSERVACION { get; set; } = string.Empty;
    [Display(Name = "Estado")]
    public int? PLD_ESTADO {  get; set; }
    public DateTime? PLD_FECHA_F {  get; set; } = DateTime.Now;
    public int? PLD_DIA_C {  get; set; }

    [Display(Name = "Empleado Asignado")]
    public int? PLD_EMP_ASIGNADO { get; set; }



}
