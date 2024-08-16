namespace PIGGISWS.Models;

public class Provincia_Agente
{
    public int ID_SECUENCIA_PK { get; set; }

    public int ID_EMPRESA_FK { get; set; }
    public int ID_AGENTE_FK { get; set; }
    public int ID_PROVINCIA_FK { get; set; }
    public int INACTIVO_NR {  get; set; }
               
        
}

