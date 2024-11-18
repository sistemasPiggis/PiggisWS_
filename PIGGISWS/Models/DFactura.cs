using System.Numerics;

namespace PIGGISWS.Models;

public class DFactura
{
    public int DFAC_EMPRESA {  get; set; }
    public decimal DFAC_CFAC_COMPROBA {  get; set; }
    public decimal DFAC_CANTIDAD { get; set; }
    public decimal DFAC_TOTAL {  get; set; }
    public int DFAC_SECUENCIA { get; set; }
    public int DFAC_PRODUCTO { get; set; }
}
