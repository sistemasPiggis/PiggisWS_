using Microsoft.Graph;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Security.Policy;

namespace PIGGISWS.Models;

public class Ccomfaci
{
    public int CFAC_EMPRESA {  get; set; }
    public decimal CFAC_CCO_COMPROBA { get; set; }
    public int? CFAC_AUTORIZA { get; set; }
    public long? CFAC_POLITICA { get; set; }
    public decimal? CFAC_LISTA_PRECIOS { get; set; }
    public int? CFAC_EST_ENTREGA { get; set; }
    public int? CFAC_PROC_FAC {  get; set; }
    public int? CFAC_PROCESO {  get; set; }
    public string? CFAC_NOMBRE { get; set; } = string.Empty;
    public string? CFAC_DIRECCION { get; set; } = string.Empty;
    public string? CFAC_TELEFONO {  get; set; } = string.Empty;
    public string? CFAC_CED_RUC {  get; set; } = string.Empty;
    public int? CFAC_TIPO_ACTPRO { get; set; }
    public int? CFAC_SOL_COMPROBA { get; set; }
    public int? CFAC_IMPUESTO { get; set; }
    public decimal? CFAC_PORC_IMPUESTO { get; set; }
    public DateTime? CFAC_FECHA_FAC { get; set; }

    public int? CFAC_TIPOPAGO { get; set; }
    public int? CFAC_COMISION { get; set; }
    public int? CFAC_IMPRIMIO { get; set; }
    public long? CFAC_CIUDAD {  get; set; }
    //public int CFAC_ORDEN { get; set; }
    //public int CFAC_PEDIDO { get; set;  }
}
