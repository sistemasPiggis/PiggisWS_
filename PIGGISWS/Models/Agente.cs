using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PIGGISWS.Models;

[Index(nameof(AGE_EMPRESA), nameof(AGE_CODIGO), IsUnique = true)]
public class Agente
{
    [Display(Name = "Código de Empresa")]
    public int AGE_EMPRESA { get; set; }
    [Display(Name = "Código de Agente")]
    public int AGE_CODIGO { get; set; }

    [Display(Name = "Nombre Agente")]

    [StringLength(15, MinimumLength = 10, ErrorMessage = "AGE_TIPO debe tener entre 10 y 15 caracteres.")]
    public string AGE_NOMBRE { get; set; } = string.Empty;
    [Display(Name = "Código de ID")]
    public string AGE_ID { get; set; } = string.Empty;

    public int AGE_PORC_COMISION { get; set; }

    [Display(Name = "Código de Empleado")]
    public int AGE_EMPLEADO { get; set; }

    [Display(Name = "Estado de Agente")]
    public int AGE_INACTIVO { get; set; }

    public string CREA_USR { get; set; }= string.Empty;

    public DateTime CREA_FECHA {  get; set; }

    public string MOD_USR {  get; set; } = string.Empty;
    public DateTime MOD_FECHA { get; set; }

    [Display(Name = "Bodega de Agente")]
    public int AGE_BODEGA { get; set; }

    public int AGE_CLIENTE { get; set; }

    public int AGE_CUEGASTO { get; set; }
    [Display(Name = "Almacen de Agente")]
    public int AGE_ALMACEN {  get; set; }
    public int AGE_PVENTA { get; set; }

    public int AGE_ALMACEN1 { get; set; }
    public int AGE_PVENTA1 { get; set; }
    [Display(Name = "Departamento Agente")]
    public int AGE_DEPARTAMENTO { get; set; }
    [Display(Name = "Ubicación de Agente")]
    public int AGE_UBICACION { get; set; }
    [Display(Name = "Código de Agente al que reporta")]
    public int AGE_REPORTA { get; set; }
    [Display(Name = "Mail de Agente")]
    public string AGE_MAIL { get; set; } = string.Empty;
    [Display(Name = "Teléfono de Agente")]
    public int AGE_TELEFONO { get; set; }
    public int AGE_REMPLAZA { get; set; }
}
