using System.Security.Policy;

namespace PIGGISWS.Models.Vistas;

public class Rep_Cons_Cartera_Interneta
{
    public int? CCO_TIPODOC { get; set; }
    public int? CCO_SIGLA { get; set; }

    public int? DDO_DEBCRE { get; set; }
    public decimal? DDO_CODCLIPRO { get; set; }
    public string? CLI_NOMBRE { get; set; } = string.Empty;
    public string? DOC { get; set; } = string.Empty;
    public decimal? DDO_MONTO { get; set; }
    public DateTime? DDO_FECHA_VEN { get; set; }
    public DateTime DDO_FECHA_EMI { get; set; }
    public decimal? CANCELA { get; set; }
    public decimal? SALDO { get; set; }
    public decimal? SALDOT { get; set; }
     

}
