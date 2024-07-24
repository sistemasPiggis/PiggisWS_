using Microsoft.EntityFrameworkCore;

namespace PIGGISWS.Models;

[Index(nameof(CDI_EMPRESA), nameof(CDI_CLIENTE), nameof(CDI_DIA), IsUnique = true)]
public class Cliente_dia
{
    
    public int CDI_EMPRESA {  get; set; }
    public int CDI_CLIENTE { get; set; }
    public string? CDI_DIA { get; set; } = string.Empty;
}
