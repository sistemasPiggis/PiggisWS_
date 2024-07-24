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


    [Display(Name = "Código de Empleado")]
    public int AGE_EMPLEADO { get; set; }

    [Display(Name = "Estado de Agente")]
    public int? AGE_INACTIVO { get; set; }

    
    [Display(Name = "Bodega de Agente")]
    public int? AGE_BODEGA { get; set; }

   
    [Display(Name = "Departamento Agente")]
    public int? AGE_DEPARTAMENTO { get; set; }
   
    [Display(Name = "Código de Agente al que reporta")]
    public int? AGE_REPORTA { get; set; }
    [Display(Name = "Mail de Agente")]
    public string AGE_MAIL { get; set; } = string.Empty;
    [Display(Name = "Teléfono de Agente")]
    public int? AGE_TELEFONO { get; set; }
}
