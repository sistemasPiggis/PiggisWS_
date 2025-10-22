namespace PIGGISWS.Models;

public class GEN_MENSAJERIA
{
    public int ID_MENSAJE { get; set; }
    public string? DESTINO_SMS { get; set; } = string.Empty;
    public string? DESTINO_CORREO { get; set; } = string.Empty;
    public string? MENSAJE_SMS { get; set; } = string.Empty;
    public int? ESTADO_SMS { get; set; }
    public int? ESTADO_CORREO { get; set; }
    public string? MAC_ADDRESS { get; set; } = string.Empty;
    public string? MODULO { get; set; } = string.Empty;
    public int? ENVIA_SMS { get; set; }
    public int? ENVIA_CORREO { get; set; }
    public string?  MENSAJE_CORREO_RES { get; set; } = string.Empty;
    public string? ASUNTO_CORREO { get; set; } = string.Empty;
    public string? QUIEN_ENVIA_CORREO { get; set; } = string.Empty;
    public string? RESPONDER_CORREO_A { get; set; } = string.Empty;
    public string? TIPO { get; set; } = string.Empty;
    public string? ADJUNTOS { get; set; } = string.Empty;
    public string? PROCESO_ENVIO { get; set; } = string.Empty;
    public string? MENSAJE_CORREO { get; set; } = string.Empty;
    public string? CREA_USR { get; set; } = string.Empty;
    public DateTime? CREA_FECHA { get; set; }
    public string? MOD_USR { get; set; }
    public DateTime? MOD_FECHA { get; set; } = DateTime.Now;
}
