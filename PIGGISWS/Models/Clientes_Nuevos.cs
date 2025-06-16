using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Clientes_Nuevos
{
    public int ID_SECUENCIA_PK { get; set; }
    [Required]
    public int ID_EMPRESA_FK { get; set; }
    [Required]
    public long ID_AGENTE_FK { get; set; }
    [Required]
    public string CI_RUC {  get; set; } = string.Empty;
    [Required]
    public string NOMBRE_CLIENTE { get; set; } = string.Empty;
    [Required]
    public string NOMBRE_COMERCIAL { get; set; } = string.Empty;
    [Required]
    public string TELEFONO { get; set; } = string.Empty;
    [Required]
    public string EMAIL { get; set; } = string.Empty;
    [Required]
    public int ID_PROVINCIA_FK { get; set; }
    [Required]
    public int ID_CANTON_FK { get; set; }
    [Required]
    public int ID_ZONA_FK { get; set; }
    [Required]
    public string DIRECCION_CLIENTE { get; set; } = string.Empty;
    public int ID_ESTABLECIMIENTO_FK { get; set; }
    public long ID_LISTA_PRECIO_FK { get; set; }

    public string ESTADO {  get; set; }= string.Empty;
    [Required]

    public long ID_PARROQUIA_FK { get; set; }
    [Required]
    public string DIRECCION_ENTREGA { get; set; } = string.Empty ;
    [Required]
    public int TIPO_IDENTIFICACION { get; set; }
    public decimal? LATITUD_NR {  get; set; }
    public decimal? LONGITUD_NR { get; set;}
    public DateTime? CREA_FECHA { get; set; } = DateTime.Now;

}
