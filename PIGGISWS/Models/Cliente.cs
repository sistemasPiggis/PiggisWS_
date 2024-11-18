namespace PIGGISWS.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;


[Index(nameof(CLI_AGENTE), nameof(CLI_EMPRESA), IsUnique = true)]
public class Cliente
{
    [Display(Name = "Código de Empresa")]
    public decimal CLI_EMPRESA { get; set; }
    [Key]
    [Display(Name = "Código de Cliente")]
    public decimal CLI_CODIGO { get; set; }

    public string? CLI_ID { get; set; } = string.Empty;

    public string? CLI_NOMBRE { get; set; } = string.Empty;
    public string? CLI_NOMBRECOM { get; set; } = string.Empty;
    public string? CLI_RUC_CEDULA { get; set; } = string.Empty;
    public long? CLI_CIUDAD { get; set; } 
    public decimal? CLI_ZONA { get; set; } = decimal.Zero;
    public string? CLI_DIRECCION { get; set; } = string.Empty;
    public string? CLI_TELEFONO1 { get; set; } = string.Empty;
    public string? CLI_DIR_ENTREGA { get; set; } = string.Empty;
    public long? CLI_PARROQUIA { get; set; }
    public string? CLI_MAIL { get; set; } = string.Empty;

    public decimal? CLI_CUPO { get; set; } = decimal.Zero;

    public long ? CLI_AGENTE { get; set; } 

    public decimal? CLI_LISTAPRE { get; set; }
    public int? CLI_ILIMITADO { get; set; }
    //public string? CLI_DIA { get; set; } = string.Empty;
    public long? CLI_POLITICAS { get; set; }
    public int? CLI_BLOQUEO { get; set; }
    public int? CLI_TIPO { get; set; }
    public int? CLI_INACTIVO { get; set; }
    public int? CLI_ESTABLECIMIENTO { get; set; }
}
       
