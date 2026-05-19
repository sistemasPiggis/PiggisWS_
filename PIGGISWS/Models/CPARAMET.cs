using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace PIGGISWS.Models;

public class CPARAMET
{
    public int? CPA_EMPRESA { get; set; }
    public decimal? CPA_CODIGO { get; set; }
    public string? CPA_SIGLA { get; set; } = string.Empty;
    public string? CPA_SECUENCIA { get; set; } = string.Empty;
    public string? CPA_NOMBRE { get; set; } = string.Empty;

    public string? CPA_TEXTO { get; set; } = string.Empty;
    public decimal ? CPA_VALOR { get; set; }
    public int? CPA_CUENTA { get; set; }
    public int? CPA_INACTIVO { get; set; }
}
