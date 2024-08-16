namespace PIGGISWS.Models;

public class Precio_Agente
{

    public int ID_PRECIO_AGENTE_PK {  get; set; }
    public int ID_EMPRESA_FK {  get; set; }
    public int ID_AGENTE_FK { get; set; }
    public int ID_PRECIO_FK { get; set; }
    public int INACTIVO_NR { get; set; }
}
