namespace PIGGISWS.Models.DTOs;

public class AuxNuevoPedidoNav
{

    public PedidoNavCab? PedidoNavCab { get; set; }
    public List<PedidoNavDetalle>? PedidoNavDetalle { get; set; }
    public TDS_PEDIDOS_NAV_CAB? TDS_PEDIDOS_NAV_CAB { get; set; }
    public List<TDS_PEDIDO_NAV_DET>? TDS_PEDIDO_NAV_DET { get; set; }
}
