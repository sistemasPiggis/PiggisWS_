using Microsoft.EntityFrameworkCore;

namespace PIGGISWS.Models;
[Index(nameof(UBO_BODEGA), nameof(UBO_USUARIO), nameof(UBO_EMPRESA), IsUnique = true)]
public class UsrBod
{
    public int UBO_EMPRESA { get; set; }
    public int UBO_USUARIO { get; set; }
    public virtual Usuario USUARIO { get; set; }

    public int UBO_BODEGA { get; set; }
    public virtual Bodega BODEGA { get; set; }
    public int? UBO_INACTIVO { get; set; }
    public int? UBO_DEFAULT { get; set; }

}
