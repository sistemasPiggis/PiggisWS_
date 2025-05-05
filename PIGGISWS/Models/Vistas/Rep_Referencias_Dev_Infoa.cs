using System.Drawing;

namespace PIGGISWS.Models.Vistas;

public class Rep_Referencias_Dev_Infoa
{

    public string CMO_REFERENCIA {  get; set; } = string.Empty;
    public decimal CCO_CODIGO { get; set; }
    public decimal? CCO_AGENTE { get; set; }
    public DateTime  CCO_FECHA { get; set; }
    public string CLI_NOMBRE { get; set; } = string.Empty;
    public string REFERENCIA { get; set; } = string.Empty;

}
