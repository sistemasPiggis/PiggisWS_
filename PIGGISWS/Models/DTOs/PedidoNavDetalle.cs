namespace PIGGISWS.Models.DTOs;

public class PedidoNavDetalle
{


        public int ID_PEDIDO_DET { get; set; }
        public int ID_EMPRESA { get; set; }
        public int ID_PEDIDO_FK { get; set; }
        public int PRO_CODIGO { get; set; }
        public string PRO_NOMBRE { get; set; } = string.Empty;
        public decimal CANTIDAD { get; set; }
        public decimal PRECIO { get; set; }
        public decimal UMD_CODIGO { get; set; }
        public string UMD_NOMBRE { get; set; } = string.Empty;
}
