using Microsoft.Graph;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIGGISWS.Models;

public class DFacturai
{
    public int DFAC_EMPRESA {  get; set; }
    public decimal DFAC_CFAC_COMPROBA { get; set; }
    public int DFAC_SECUENCIA { get; set; }
    public int? DFAC_PRODUCTO { get; set; }
    public decimal?  DFAC_CATPRODUCTO { get; set; }
    public decimal  DFAC_CANTIDAD {  get; set; } 
    public decimal  DFAC_CANAPR {  get; set; } 
    public decimal  DFAC_PRECIO {  get; set; } 
    public decimal? DFAC_DESCUENTO { get; set; }
    public int DFAC_BODEGA { get; set; }
    public decimal DFAC_TOTAL { get; set; } 
    public decimal DFAC_CANENT {  get; set; }
    public decimal DFAC_CANDEV {  get; set; }
    public decimal DFAC_CANRES {  get; set; } 
    public decimal DFAC_DSCITEM { get; set; }
    public decimal DFAC_TRAITEM {  get; set; } 
    public int? DFAC_COMBO {  get; set; }
    public decimal DFAC_IVAITEM { get; set; }
    public int DFAC_GRABAIVA {  get; set; } 
    public decimal DFAC_UDIGITADA { get; set; }
    public decimal DFAC_CDIGITADA { get; set; }
    public decimal? DFAC_CEQ {  get; set; }
    public int? DFAC_UEQ { get; set; }
    public int DFAC_PROMOCION { get; set; }
    public decimal DFAC_CANT_PEDIDA { get; set; }
    public decimal? DFAC_CAPRDIGITADA { get; set; }

    [NotMapped]
    public int DFAC_ESTADO {  get; set; }

    //para llevar el nombre del producto 
    [NotMapped]
    public string PRO_NOMBRE { get; set; } = string.Empty;
}
