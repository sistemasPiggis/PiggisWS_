using System.Security.Cryptography;

namespace PIGGISWS.Models.Vistas;

public class Rep_Cantidades_Pedidosa
{
    public decimal PRO_CODIGO  { get; set; }
    public string CLASIFICACION { get; set; } = string.Empty;
    public decimal CCO_BODEGA { get; set; }
    public decimal DFAC_CANTIDAD { get; set; }
    public decimal AGE_CODIGO { get; set; }
    public string PRO_ID { get; set; } = string.Empty;
    public DateTime CFAC_FECHA_FAC { get; set; }
    public string DOC { get; set; } = string.Empty;
    public string CREA_USR { get; set; } = string.Empty;
    public DateTime CCO_FECHA { get; set; }
    public string ALM_NOMBRE { get; set; } = string.Empty;
    public string ZON_NOMBRE { get; set; } = string.Empty;

    public string AGE_NOMBRE { get; set; } = string.Empty;
    public string CLI_ID { get; set; } = string.Empty;
    public string CLI_NOMBRE { get; set; } = string.Empty;
    public string PRO_NOMBRE { get; set; } = string.Empty;
    public decimal DFAC_CDIGITADA { get; set; }
    public decimal? DFAC_PDIGITADO { get; set; }
    public string UMD_ID { get; set; } = string.Empty;
    public decimal? TOTAL { get; set; }
    public string DIA { get; set; } = string.Empty;
    public decimal TOTAL_CON_DESCUENTOS { get; set; }
    public string CTI_ID { get; set; } = string.Empty;
    public string IMPUESTO { get; set; } = string.Empty;
    public decimal TOTAL_KILOS { get; set; }
    public DateTime CREA_FECHA { get; set; }
    public string CCO_DOCTRAN { get; set; } = string.Empty;
    public string? FACTURA { get; set; } = string.Empty;
    public decimal DFAC_CANT_PEDIDA { get; set; }

    public int CCO_NUMERO { get; set; }
    public decimal CLI_CODIGO { get; set; }

    public int CCO_PERIODO { get; set; }
    public int CCO_MES { get; set; }
    public int CCO_DIA { get; set; }
    public decimal? CCO_CODIGO { get; set; }

    public int? DFAC_SECUENCIA { get; set; }

}
