using System.Security.Policy;

namespace PIGGISWS.Models;

public class Notificaciones
{
    public long NOT_CODIGO { get; set; }
    public int NOT_EMPRESA { get; set; }
    public string NOT_DESCRIPCION { get; set; } = string.Empty;
    public string NOT_OBSERVACIONES { get; set; } = string.Empty;
    public int NOT_INACTIVO { get; set; }
    public string NOT_COMUNICADO { get; set; } = string.Empty;
    public int NOT_ESTADO { get; set; }
    public int NOT_TIPO { get; set; }
    public int NOT_PROCESADA { get; set; }
    public DateTime CREA_FECHA { get; set; }
    public int? APP_DESTINO { get; set; }
}
