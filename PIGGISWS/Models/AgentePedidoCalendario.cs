namespace PIGGISWS.Models;

    public class AgentePedidoCalendario
    {
    public int AGE_ID_CALENDARIO_PK {  get; set; }
    public int AGE_ID_EMPRESA_FK { get; set; }
    public decimal AGE_ID_EMPLEADO_FK { get; set; } ////id del agente, el nombre esta mal en la BD

    public string AGE_HORA_INICIO { get; set; } = "00:00";
    public string AGE_HORA_CIERRE { get; set; } = "00:00";
    public int INACTIVO_NR { get; set; }
    public string AGE_DIA { get; set; } = string.Empty;
    public int AGE_ID_ALMACEN { get; set; }
}

