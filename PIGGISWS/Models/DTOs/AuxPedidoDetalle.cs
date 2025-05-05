namespace PIGGISWS.Models.Auxiliares;

public class AuxCComproba
{
    public int CCO_EMPRESA { get; set; }
    public decimal CCO_CODIGO { get; set; } /// <summary>
    /// 
    /// </summary>
    public DateTime CCO_FECHA { get; set; }
    public string CTI_NOMBRE { get; set; }
    public decimal CLI_CODIGO { get; set; }
    //public decimal DFAC_CANTIDAD { get; set; }
    //public decimal DFAC_TOTAL { get; set; }
    public decimal DFAC_CANTIDADT { get; set; }
    public decimal DFAC_TOTALT { get; set; }
    public int CCO_NUMERO { get; set; }
    public string? CCO_DETALLE { get; set; } = string.Empty;
    public int CCO_DIA { get; set; }
    public int CCO_MES { get; set; }
    public int CCO_PERIODO { get; set; }
    public int CCO_CIE_COMPROBA { get; set; }
    public int CCO_AGENTE { get; set; }
    

}

public class AuxDFactura
{
    public int DFAC_EMPRESA { get; set; }
    public int DFAC_CFAC_COMPROBA { get; set; }
    public decimal DFAC_CANTIDAD { get; set; }
    public decimal DFAC_TOTAL { get; set; }
    public int DFAC_SECUENCIA { get; set; }
    public string PRO_NOMBRE { get; set; } = string.Empty ;
}

public class AuxPedido
{
    public AuxCComproba Cabecera { get; set; }
    public List<AuxDFactura>? Detalles { get; set; }
    public decimal TOTAL_KILOS { get; set; }
}
