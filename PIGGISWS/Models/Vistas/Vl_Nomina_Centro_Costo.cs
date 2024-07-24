namespace PIGGISWS.Models.Vistas;

public class Vl_Nomina_Centro_Costo
{
    public int ID_EMPRESA {  get; set; }
    public int ID_DEPARTAMENTO { get; set;}
    public int ID_CENTRO_PADRE { get; set; }
    public int ID_CENTRO_COSTO { get; set; }
    public string CODIGO_DEPARTAMENTO { get; set;}  = string.Empty;
    public int NOMBRE_DEPARTAMENTO { get; set;}
    public int CODIGO_CENTRO_PADRE { get; set;}
    public string NOMBRE_CENTRO_PADRE { get; set; } = string.Empty; 
    public int CODIGO_CENTRO_COSTO { get; set;}
    public string NOMBRE_CENTRO_COSTO { get; set; } = string.Empty;

}
