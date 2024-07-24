using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Map_Cerca_Agente
{
    [Key]
    public int ID_MAP_CERCA { get; set; }
    public int ID_EMPRESA { get; set; }
    public int AGE_CODIGO { get; set; }
    public string DIA {  get; set; } = string.Empty;
    public string LATITUD { get; set; } = string.Empty;
    public string LONGITUD { get; set; } = string.Empty;

    public int SECUENCIA { get; set; }
    //public string? SECTOR { get; set; } = string.Empty;
    //public int? ID_MAP_CERCA_FK { get; set; }
}
