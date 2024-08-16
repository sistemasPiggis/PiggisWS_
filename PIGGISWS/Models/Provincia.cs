namespace PIGGISWS.Models;

using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;

public class Provincia
{
    [Key]
    public int ID_PROVINCIA_PK { get; set; }
    [Display(Name = "Provincia Nombre")]
    public string NOMBRE_TX { get; set; } = string.Empty;
    [Display(Name = "Provincia Estado")]
    public int? INACTIVO_NR { get; set; }
}


