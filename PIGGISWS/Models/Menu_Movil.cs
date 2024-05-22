using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;

namespace PIGGISWS.Models;

public class Menu_Movil
{
    [Key]
    public int? MNV_CODIGO { get; set; }
    public string? MNV_ID { get; set; } = string.Empty;

    public string? MNV_NOMBRE { get; set; } = string.Empty;


    [ForeignKey("MenuId")]
    public int? MNV_REPORTA { get; set; } 
    

    public int? MNV_ORDEN { get; set; }
    public string? MNV_ACTIVIDAD { get; set; } = string.Empty;
    public string? CREA_USR { get; set; } = string.Empty;

    public DateTime? CREA_FECHA { get; set; }
    public string? MOD_USR { get; set; } = string.Empty;


    public DateTime? MOD_FECHA { get; set; }
    public int? MNV_INACTIVO { get; set; }

    public string? MNV_TIPO_ACTIVIDAD { get; set; } = string.Empty;
    public string? MNV_MODO_APP { get; set; } = string.Empty;

    public string? MNV_TIPO_APP { get; set; } = string.Empty;

    public string? MNV_IMAGEN { get; set; } = string.Empty;  


}
