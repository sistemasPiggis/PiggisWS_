namespace PIGGISWS.Models;

using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;

public class Provincia
{
    [Key]
    public int ID_PROVINCIA_PK { get; set; }

    public int ID_PAIS_FK { get; set; }
    [Display(Name = "Provincia Nombre")]
    public string NOMBRE_TX { get; set; } = string.Empty;
    public string CODIGO_REFERENCIA_TX { get; set; } = string.Empty;   
    public string CREA_USR { get; set; } = string.Empty;

    public DateTime CREA_FECHA { get; set; }
    public string MOD_USR { get; set; } = string.Empty;

    public DateTime MOD_FECHA { get; set; }
    [Display(Name = "Provincia Estado")]
    public int INACTIVO_NR { get; set; }
}


