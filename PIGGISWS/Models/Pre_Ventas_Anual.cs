namespace PIGGISWS.Models;

public class Pre_Ventas_Anual
{
    public int PVA_CODIGO_PK { get; set; }
    public int PVA_EMPRESA_FK { get; set; }
    public decimal PVA_COD_AGENTE_FK { get; set; }
    public int PVA_PERIODO_NR { get; set; }
    public int PVA_MES_NR { get; set; }
    public string PVA_CALIDAD_TX { get; set; } = string.Empty;
    public decimal? PVA_VALOR_NR { get; set; }
    public string? PVA_TIPO_PERSONA { get; set; } = string.Empty;
}
