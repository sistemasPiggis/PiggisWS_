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

    public int? PLD_EMP_SOLICITA { get; set; }
    public int? PLD_EMP_FINALIZA { get; set; }
    public string? PLD_OBSERVACION { get; set; } = string.Empty;
    public int? PLD_ESTADO {  get; set; }
    public DateTime? PLD_FECHA_F {  get; set; } = DateTime.Now;
    public int? PLD_DIA_C {  get; set; }
    public int? PLD_EMP_ASIGNADO { get; set; }



}
