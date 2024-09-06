using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace PIGGISWS.Models;

public class Zona
{

    public int ZON_EMPRESA {  get; set; }
    public int ZON_CODIGO { get; set; }
    public string ZON_ID { get; set;} = string.Empty;
    public string ZON_NOMBRE { get; set; } = string.Empty;
    public int? ZON_INACTIVO { get; set; }
    public int? ZON_ORDEN {  get; set; }
    public int ZON_CANTON_FK { get; set; }

}
