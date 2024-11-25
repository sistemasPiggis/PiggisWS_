using Microsoft.Graph;

namespace PIGGISWS.Models.Vistas;

public class Rep_Motivos_Dev
{
    public string TDE_NOMBRE {  get; set; } =string.Empty;
    public decimal TDE_CODIGO { get; set; }
    public int TDE_EMPRESA { get; set; }
    public string TDE_ID { get; set; } = string.Empty;
    public int? TDE_INACTIVO { get; set; }
    public decimal TDE_REPORTA { get; set; }
}
