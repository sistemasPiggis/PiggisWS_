using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;
[Index(nameof(UBO_BODEGA), nameof(UBO_USUARIO), nameof(UBO_EMPRESA), IsUnique = true)]
public class UsrBod
{
    [Display(Name = "Empresa")]
    public int UBO_EMPRESA { get; set; }
    [Display(Name = "Usuario")]
    public int UBO_USUARIO { get; set; }
    public virtual Usuario USUARIO { get; set; }
    [Display(Name = "Bodega")]
    public int UBO_BODEGA { get; set; }
    public virtual Bodega BODEGA { get; set; }
    [Display(Name = "Estado")]
    public int? UBO_INACTIVO { get; set; }
    public int? UBO_DEFAULT { get; set; }

}
