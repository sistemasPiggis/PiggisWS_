namespace PIGGISWS.Models.Vistas;

public class Vl_Ncc_Idc_ldv
{
    public int ID_EMPRESA_NR { get; set; }
    public decimal ID_IDC_NR { get; set; }
    public string NUMERO_NOTA_CREDITO {  get; set; } = string.Empty;
    public string NUMERO_IDC { get; set; } = string.Empty;
    public string NUMERO_LDV { get; set; } = string.Empty;
    public string NUMERO_FACTURA {  get; set; } = string.Empty;
    public decimal? TOTAL_NOTA_CREDITO { get; set; }
}
