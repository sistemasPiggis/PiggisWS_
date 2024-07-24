using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIGGISWS.Models;

public class Parametros_Movil
{

    public int CODIGO { get; set; }
    public string NOMBRE { get; set; } = string.Empty;
    public string TIPO { get; set; } = string.Empty;
    public string VALOR { get; set; } = string.Empty;
    public string DESCRIPCION { get; set; } = string.Empty;
    public string MODULO { get; set; } = string.Empty;
    public string SERVICIO { get; set; } = string.Empty;

}

