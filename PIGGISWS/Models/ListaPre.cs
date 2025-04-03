using System.Security.Policy;

namespace PIGGISWS.Models;

public class ListaPre
{
    public int LPR_EMPRESA {  get; set; }
    public long LPR_CODIGO { get; set; }
    public string LPR_NOMBRE {  get; set; } = string.Empty;
    public string? LPR_ID {  set; get; } = string.Empty;
    public int? LPR_MONEDA { get; set; }
    public int? LPR_INACTIVO { get; set; }
    public int? LPR_NUMERO {  get; set; }
    //public DateTime? DLP_FECHA_INI { get; set; }
}
