using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PIGGISWS.Models;

[Index(nameof(USR_ID), IsUnique = true)]
public class Usuario
{
    [Key]
    [Display(Name = "Usuario Codigo")]
    public int USR_CODIGO {  get; set; }
    [Display(Name = "Usuario ID")]
    public string USR_ID { get; set; } = string.Empty;
    [Display(Name = "Usuario Nombre")]
    public string USR_NOMBRE { get; set; } = string.Empty;
    public string USR_CLAVE { get; set; } = string.Empty;
    [Display(Name = "Usuario Cliente")]
    public int? USR_CLIENTE { get; set; }
    [Display(Name = "Usuario Agente")]
    public int? USR_AGENTE { get; set; }

    [Display(Name = "Usuario Empleado")]
    public int? USR_EMPLEADO { get; set; }


}
