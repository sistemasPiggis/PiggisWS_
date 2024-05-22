namespace PIGGISWS.Models;

using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;

public class Provincia
{
    [Key]
    public int ID_PROVINCIA_PK { get; set; }

    public int ID_PAIS_FK { get; set; }

    public string NOMBRE_TX { get; set; }
    public string CODIGO_REFERENCIA_TX { get; set; }
    public string CREA_USR { get; set; }

    public DateTime CREA_FECHA { get; set; }
    public string MOD_USR { get; set; }

    public DateTime MOD_FECHA { get; set; }
    public int INACTIVO_NR { get; set; }
}


