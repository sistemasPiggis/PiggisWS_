namespace PIGGISWS.Models;

public class TipoDev
{
    public int TDE_EMPRESA { get; set; }

    public int TDE_CODIGO { get; set; }

    public string TDE_ID { get; set; } =string.Empty;
    public string TDE_NOMBRE { get; set; } = string.Empty;
    public int TDE_INACTIVO { get; set; }
    public int TDE_REPORTA { get; set; }
    public int TDE_ORDEN {  get; set; }
}
