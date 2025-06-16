using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace PIGGISWS.Models.DTOs;

public class AuxClienteApp
{
    public int CLI_EMPRESA { get; set; }
    public int CLI_CODIGO { get; set; }
    public string CLI_ID { get; set; } = string.Empty;
    public string CLI_NOMBRE { get; set; } = string.Empty;
    public string CLI_NOMBRECOM { get; set; } = string.Empty;
    public long CLI_AGENTE { get; set; }
    public int CLI_LISTAPRE { get; set; }
    public int CLI_ILIMITADOF { get; set; }
    public long CLI_ZONA { get; set; }
    public string CDI_DIA { get; set; } = string.Empty;
    public string CLI_TELEFONO1 { get; set; } = string.Empty;
    public int CLI_POLITICAS { get; set; }
    public string CLI_DIRECCION { get; set; } = string.Empty;
    public string CLI_DIR_ENTREGA { get; set; } = string.Empty;
    public string CLI_MAIL { get; set; } = string.Empty;
    public string CLI_RUC_CEDULA { get; set; } = string.Empty;
    public decimal CLI_CIUDAD { get; set; }
    public decimal POL_PORC_DESC { get; set; }
    public decimal POL_PORC_FINANC { get; set; }
    public decimal POL_PORC_PRO_PAGO { get; set; }
    public decimal POL_PORC_PAG_CONTA { get; set; }
    public decimal POL_LINEA_CREDITO { get; set; }
    public long ID_PROVINCIA_FK { get; set; }
    public long ID_CANTON_FK { get; set; }
    public int POL_DIAS_PLAZO { get; set; }
    public int POL_NRO_PAGOS { get; set; }
    public decimal CUPO { get; set; }
    public decimal DEUDA { get; set; }
    public long? CLI_PARROQUIA { get; set; }
    public int? CLI_ESTABLECIMIENTO { get; set; }

    public decimal DISPONIBLE { get; set; }
    public DateTime? FECHA_SUG { get; set; }


    private int _cliBloqueo;
    public int CLI_BLOQUEO
    {
        get => _cliBloqueo;
        set
        {
           
        }
    }

    public decimal CLI_LATITUD_NR { get; set; }
    public decimal CLI_LONGITUD_NR { get; set; }
    public decimal CLI_LATITUD1_NR { get; set; }
    public decimal CLI_LONGITUD1_NR { get; set; }

}
