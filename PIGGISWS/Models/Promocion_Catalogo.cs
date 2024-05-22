using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Promocion_Catalogo
{
    [Key]
    public int ID_PROMOCION_CATALOGO_PK { get; set; }

    [Display(Name = "Código de Empresa")]

    public int ID_EMPRESA_FK { get; set; } = 1;
    [Display(Name = "Descripción Promoción")]
    public string DESCRIPCION_TX { get; set; } = string.Empty;
    [Display(Name = "Imagen")]
    public string IMAGEN_TX { get; set; } = string.Empty;

    public string FILENAME { get; set; } = string.Empty;

    public string MIME_TYPE { get; set; } = string.Empty;

    public int DOC_SIZE { get; set; }

    public string DAD_CHARSET { get; set; } = string.Empty;

    public string CREA_USR { get; set; } = string.Empty;

    public DateTime CREA_FECHA { get; set; } = DateTime.Now;

    public string MOD_USR { get; set; } = string.Empty;

    public DateTime MOD_FECHA { get; set; } = DateTime.Now;

    public int? INACTIVO_NR { get; set; } = 0;

    public int ORDEN_NR { get; set; }

    public int? APP_AGENTES { get; set; } = 0;


}
