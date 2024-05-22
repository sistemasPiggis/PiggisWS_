using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Gen_Noticias
{
    [Key]
    public int  NOT_CODIGO {  get; set; }
    [Display(Name = "Noticia Titulo")]
    public string? NOT_TITULO { get; set; } = string.Empty;
    [Display(Name = "Noticia Descripción")]
    public string? NOT_DESCRIPCION {  get; set; } = string.Empty;
    [Display(Name = "Noticia Imagen")]
    public string? NOT_IMAGEN {  get; set; } = string.Empty;
    [Display(Name = "Noticia Estado")]
    public string? NOT_ESTADO {  get; set; } = string.Empty;
    [Display(Name = "Noticia Fecha Creación")]
    public DateTime CREA_FECHA {  get; set; } = DateTime.Now;
    public string CREA_USR { get; set; } = string.Empty;
    public DateTime MOD_FECHA { get; set; } = DateTime.Now;
    public string MOD_USR { get; set;} = string.Empty;
    public string NOT_FILENAME {  get; set; } = string.Empty;
    public string NOT_MIME_TYPE { get; set;} = string.Empty;
    public string NOT_DOC_SIZE { get;set; } = string.Empty;
    public string NOT_DAD_CHARSET {  get; set; } = string.Empty;
    public int NOT_EMPRESA { get; set; }

    public int NOT_TIPO { get; set; } = 0;
    [Display(Name = "Noticia Fecha Desde:")]
    public DateTime NOT_FECHA_DESDE {  get; set; } = DateTime.Now;

    [Display(Name = "Noticia Fecha Hasta:")]
    public DateTime NOT_FECHA_HASTA { get; set; } = DateTime.Now;
    public int? NOT_ORDEN { get; set; } = 0;



}
