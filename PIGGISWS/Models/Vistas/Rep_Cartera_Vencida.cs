namespace PIGGISWS.Models.Vistas;

public class Rep_Cartera_Vencida
{
     public string CLI_NOMBRE {  get; set; } =string.Empty;
     public int CLI_CODIGO { get; set; }
     public int AGE_CODIGO { get; set; }
     public string AGE_NOMBRE { get; set; } = string.Empty;
     public int  POL_DIAS_PLAZO { get; set; }
     public int  CLI_EMPRESA { get; set; }
     public int CLI_AGENTE { get; set; }
     public int  DDO_CODCLIPRO { get; set; }
     public string DOC { get; set; } = string.Empty;
     public DateTime DDO_FECHA_VEN {  get; set; }
     public int DIAS_VENCIDOS { get; set; }
     public int SALDO {  get; set; }
     public DateTime DDO_FECHA_EMI { get; set; }
     public string CLI_MAIL { get; set; } = string.Empty;
     public string AGE_MAIL { get; set; } = string.Empty;

}
