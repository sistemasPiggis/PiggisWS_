using System.ComponentModel.DataAnnotations.Schema;

namespace PIGGISWS.Models;

    public class Totali
    {

    public int TOT_EMPRESA {  get; set; }

    public decimal TOT_CCO_COMPROBA { get; set; }
    public decimal? TOT_IMPUESTO {  get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TOT_PORC_DESC { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TOT_PORC_FINANC { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TOT_PORC_PRO_PAGO { get; set; }
    public decimal? TOT_PORC_PAG_CONTA {  get; set; }
    public int? TOT_LINEA_CREDITO { get; set; }
    public int? TOT_DIAS_PLAZO { get; set; }
    public int? TOT_NRO_PAGOS { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_SUBTOTAL { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_DESCUENTO1 { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_DESCUENTO2 { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_TIMPUESTO { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_TRANSPORTE { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_SEGURO_TRANS { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_AJUSTE { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_FINANCIA { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal TOT_TOTAL { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TOT_PORC_IMPUESTO { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal? TOT_DESC1_0 { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal? TOT_DESC2_0 { get; set; }
    [Column(TypeName = "decimal(17, 2)")]
    public decimal? TOT_SUBTOT_0 { get; set; }

}

