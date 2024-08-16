namespace PIGGISWS.Models;

public class Canton
{
    public int ID_CANTON_PK { get; set; }
    public int ID_PROVINCIA_FK { get; set; }
    public string NOMBRE_TX { get; set; } = string.Empty;
    public int? INACTIVO_NR { get; set; }


}
