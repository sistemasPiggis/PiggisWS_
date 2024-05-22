using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PIGGISWS.Models;

[Index(nameof(USR_ID), IsUnique = true)]
public class Usuario
{
    [Key]
    public int USR_CODIGO {  get; set; }
    
    public string USR_ID { get; set; } = string.Empty;
    public string USR_NOMBRE { get; set; } = string.Empty;
    public string USR_CLAVE { get; set; } = string.Empty;
    public int? USR_CLIENTE { get; set; }
    public int? USR_AGENTE { get; set; }
    public int? USR_EMPLEADO { get; set; }


}
