using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Banco_App
{
    [Key]
    public int BAN_CODIGO { get; set; }
    [Display(Name = "Nombre Banco")]
    public string BAN_NOMBRE { get; set; } = string.Empty;
    [Display(Name = "Banco Estado")]
    public int? BAN_INACTIVO { get; set; }
}
