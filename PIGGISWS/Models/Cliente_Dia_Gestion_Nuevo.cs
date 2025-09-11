namespace PIGGISWS.Models;

public class Cliente_Dia_Gestion_Nuevo
{

    public int ID_SECUENCIA_PK {  get; set; }
    public int ID_EMPRESA_FK { get; set; }
    public string TIPO_GESTION_TX { get; set; } = string.Empty;
    public string ID_CLIENTE_NUEVO_FK { get; set; } = string.Empty;
    public string DIA_NR { get; set; } = string.Empty;
    public int INACTIVO_NR { get; set; } = 0;
    public string DIRECCION_CLIENTE { get; set; }  =string.Empty;
    public string DIRECCION_ENTREGA { get; set; } = string.Empty;
    public string? CREA_USR { get; set; }
    public DateTime? CREA_FECHA { get; set; } = DateTime.Now;
    public string? MOD_USR { get; set; }
    public DateTime? MOD_FECHA { get; set; } = DateTime.Now;

}
