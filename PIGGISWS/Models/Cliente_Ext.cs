namespace PIGGISWS.Models;

public class Cliente_Ext
{

    public int CLI_EMPRESA { get; set; }
    public int CLI_CODIGO { get; set; }
    //public long? CLI_LATITUD_NR { get; set; }
    //public long? CLI_LONGITUD_NR { get; set; }
    public int? ID_PROVINCIA_FK { get; set; } = 0;
    public int? ID_CANTON_FK { get; set; } 
    public decimal? CLI_LATITUD_NR { get; set; }
    public decimal? CLI_LONGITUD_NR { get; set; }
    public decimal? CLI_LATITUD1_NR { get; set; }
    public decimal? CLI_LONGITUD1_NR { get; set; }
}
