namespace PIGGISWS.Models;

public class Politica
{
    public int POL_EMPRESA { get; set; }
    public int POL_CODIGO { get; set; } 
    public decimal POL_PORC_DESC { get; set; }
    public decimal POL_PORC_FINANC { get; set; }
    public decimal POL_PORC_PRO_PAGO { get; set; }
    public decimal POL_PORC_PAG_CONTA { get; set; }
    public int POL_LINEA_CREDITO { get; set; }
    public int POL_DIAS_PLAZO { get; set; }
    public int POL_NRO_PAGOS { get; set; }
    
}