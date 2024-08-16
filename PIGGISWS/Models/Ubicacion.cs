using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace PIGGISWS.Models;

public class Ubicacion
{
    public int UBI_EMPRESA {  get; set; }
    public long UBI_CODIGO { get; set; }
    public string UBI_NOMBRE {  get; set; } = string.Empty;
    public int? UBI_REPORTA { get; set; }
    public int? UBI_ORDEN {  get; set; }
    public int? UBI_INACTIVO { get; set; }
    public int? UBI_ID_PROVINCIA { get; set; }
}
