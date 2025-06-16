using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace PIGGISWS.Services;

public class PedidoService : IPedidoService
{

    private readonly ApplicationDbContext _context;

    private readonly IProductoService _productoService;
    private readonly IRuteroService _ruteroService;
    private readonly IAgenteService _agenteService;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();

    #region Prop
    int p_empresa;
    //int p_cli_Tipo = 0;
    //int p_cli_Bloqueo = 0;
    int p_cli_Inactivo = 0;
    int p_ped_sigla = 0;
    //int p_cli_cupo = 0;
    string p_ped_doctran = "";
    int p_ped_tipodoc = 0;
    int p_ped_modulo = 0;
    int p_ped_notcontable = 0;
    int p_ped_estado = 0;
    int p_ped_descuadre = 0;
    int p_ped_serie = 0;
    int p_ped_centro = 0;
    int p_ped_tclipro = 0;
    int p_ped_tipocambio = 0;
    int p_ped_anulado = 0;
    int p_ped_trasacc = 0;
    int p_ped_ccomfa_autoriza = 100;
    int p_ped_est_entrega = 0;
    int p_ped_proc_fac = 0;
    int p_ped_cfac_proceso = 0;
    int p_ped_tipo_actpro = 0;
    int p_ped_sol_comproba = 0;
    int p_ped_tipopago = 0;
    int p_ped_comision = 0;
    int p_ped_imprimio = 0;
    int p_pedd_catproducto;
    int p_pedd_canapr;
    int p_pedd_candev;
    int p_pedd_canres;
    int p_pedd_dscitem;
    int p_pedd_traitem;
    int p_pedd_combo;
    int p_pedd_ivaitem;
    int p_pedd_estado;
    int p_pedd_totimpuesto;
    int p_ped_cfac_orden;
    #endregion
    public PedidoService(ApplicationDbContext context, IProductoService productoService, IRuteroService ruteroService, IAgenteService agenteService)
    {
        _productoService = productoService;
        _ruteroService = ruteroService;
        _agenteService = agenteService;
        _context = context;
        GetParametros();
    }
    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "ProductoService" || p.SERVICIO == "GENERAL" || p.SERVICIO == "PedidoService").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_cli_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 6)?.VALOR ?? "0");
            p_ped_sigla = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 22)?.VALOR ?? "0");
            p_ped_serie = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 23)?.VALOR ?? "0");
            p_ped_doctran = parametros.First(p => p.CODIGO == 24)?.VALOR ?? "";
            p_ped_tipodoc = Convert.ToInt16(parametros.FirstOrDefault(p => p.CODIGO == 25)?.VALOR ?? "0");
            p_ped_modulo = Convert.ToInt16(parametros.FirstOrDefault(p => p.CODIGO == 26)?.VALOR ?? "0");
            p_ped_notcontable = Convert.ToInt16(parametros.FirstOrDefault(p => p.CODIGO == 27)?.VALOR ?? "0");
            p_ped_estado = Convert.ToInt16(parametros.FirstOrDefault(p => p.CODIGO == 28)?.VALOR ?? "0");
            p_ped_descuadre = Convert.ToInt16(parametros.FirstOrDefault(p => p.CODIGO == 29)?.VALOR ?? "0");
            p_ped_centro = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 30)?.VALOR ?? "0");
            p_ped_tipocambio = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 31)?.VALOR ?? "0");
            p_ped_tclipro = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 32)?.VALOR ?? "0");
            p_ped_trasacc = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 33)?.VALOR ?? "0");
            p_ped_anulado = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 34)?.VALOR ?? "0");
            p_ped_ccomfa_autoriza = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 35)?.VALOR ?? "0");
            p_ped_est_entrega = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 36)?.VALOR ?? "0");
            p_ped_proc_fac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 37)?.VALOR ?? "0");
            p_ped_cfac_proceso = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 38)?.VALOR ?? "0");
            p_ped_tipo_actpro = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 39)?.VALOR ?? "0");
            p_ped_sol_comproba = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 40)?.VALOR ?? "0");
            p_ped_tipopago = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 41)?.VALOR ?? "0");
            p_ped_comision = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 42)?.VALOR ?? "0");
            p_ped_imprimio = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 43)?.VALOR ?? "0");
            p_pedd_catproducto = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 44)?.VALOR ?? "0");
            p_pedd_canapr = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 45)?.VALOR ?? "0");
            p_pedd_candev = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 46)?.VALOR ?? "0");
            p_pedd_canres = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 47)?.VALOR ?? "0");
            p_pedd_dscitem = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 48)?.VALOR ?? "0");
            p_pedd_traitem = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 49)?.VALOR ?? "0");
            p_pedd_combo = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 50)?.VALOR ?? "0");
            p_pedd_ivaitem = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 51)?.VALOR ?? "0");
            p_pedd_estado = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 52)?.VALOR ?? "0");
            p_pedd_totimpuesto = Convert.ToInt32(parametros.FirstOrDefault(P => P.CODIGO == 53)?.VALOR ?? "0");
            p_ped_cfac_orden = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 54)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task<ServiceResponse<object>> GetPedidosxClienteCorte(decimal cliente)
    {

        var response = new ServiceResponse<object>();
        var siglapedidod = new List<int> { 90000855, 180001667 };
        var p_fecha_pedido = DateTime.Today.AddDays(-120);
        try
        {


            var pedidos = await (from cc in _context.CCOMPROBA
                                 join df in _context.DFACTURA on cc.CCO_CODIGO equals df.DFAC_CFAC_COMPROBA
                                 join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO
                                 where cc.CCO_EMPRESA == p_empresa
                                 && cc.CCO_ESTADO != 9
                                 && siglapedidod.Contains(cc.CCO_SIGLA)
                                 && cc.CCO_FECHA >= p_fecha_pedido
                                 && cc.CCO_CODCLIPRO == cliente
                                 group new { cc, df } by new { cc.CCO_NUMERO, cc.CCO_FECHA, ct.CTI_NOMBRE, cc.CCO_DETALLE, //cc.CCO_CODIGO, 
                                     cc.CCO_DIA, cc.CCO_PERIODO, cc.CCO_CIE_COMPROBA, cc.CCO_AGENTE, cc.CCO_MES 
                                 } into g
                                 select new
                                 {
                                     CCO_NUMERO = g.Key.CCO_NUMERO,
                                     CCO_FECHA = g.Key.CCO_FECHA,
                                     CTI_NOMBRE = g.Key.CTI_NOMBRE,
                                     //CCO_CODIGO = g.Key.CCO_CODIGO,
                                     DFAC_CANTIDAD = g.Sum(x => x.df.DFAC_CANTIDAD),
                                     DFAC_TOTAL = g.Sum(x => x.df.DFAC_TOTAL),
                                     CCO_DETALLE = g.Key.CCO_DETALLE,
                                     CCO_DIA = g.Key.CCO_DIA,
                                     CCO_PERIODO = g.Key.CCO_PERIODO,
                                     CCO_MES = g.Key.CCO_MES,
                                     //CCO_CIE_COMPROBA = g.Key.CCO_CIE_COMPROBA,
                                     CCO_AGENTE = g.Key.CCO_AGENTE
                                 }
                                ).OrderByDescending(o=>o.CCO_FECHA).ToListAsync();



            




            if (pedidos == null || !pedidos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            response.Data = pedidos;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            // Log the exception details (ex) here as needed
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }



    public async Task<ServiceResponse<object>> GetPedidosDetalle(AuxPedido auxPedidos)
    {

        var response = new ServiceResponse<object>();
        var siglapedidod = new List<int> { 90000855, 180001667 };


        try
        {
            var result = await (from cc in _context.CCOMPROBA
                                join df in _context.DFACTURA on cc.CCO_CODIGO equals df.DFAC_CFAC_COMPROBA
                                join pr in _context.PRODUCTO on df.DFAC_PRODUCTO equals pr.PRO_CODIGO
                                join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO
                                where cc.CCO_EMPRESA == p_empresa
                                && cc.CCO_ESTADO != 9
                                && siglapedidod.Contains(cc.CCO_SIGLA)
                                && cc.CCO_NUMERO == auxPedidos.Cabecera.CCO_NUMERO
                                && cc.CCO_PERIODO == auxPedidos.Cabecera.CCO_PERIODO
                                && cc.CCO_DIA == auxPedidos.Cabecera.CCO_DIA
                                && cc.CCO_AGENTE == auxPedidos.Cabecera.CCO_AGENTE
                                && cc.CCO_MES == auxPedidos.Cabecera.CCO_MES
                                select new
                                {
                                    cc.CCO_NUMERO,
                                    cc.CCO_FECHA,
                                    ct.CTI_NOMBRE,
                                    cc.CCO_DETALLE,
                                    cc.CCO_CODIGO,
                                    cc.CCO_DIA,
                                    cc.CCO_PERIODO,
                                    cc.CCO_CIE_COMPROBA,
                                    cc.CCO_AGENTE,
                                    cc.CCO_MES,
                                    df.DFAC_CANTIDAD,
                                    df.DFAC_TOTAL,
                                    df.DFAC_SECUENCIA,
                                    pr.PRO_NOMBRE
                                }).ToListAsync();



            //var result = await (from v in _context.REP_CANTIDADES_PEDIDOSA
            //                     join cc in _context.CCOMPROBA on v.CCO_CODIGO equals cc.CCO_CODIGO
            //                     join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO


            //                     where v.CCO_NUMERO == auxPedidos.Cabecera.CCO_NUMERO
            //                     && v.CCO_PERIODO == auxPedidos.Cabecera.CCO_PERIODO 
            //                     && v.CCO_DIA == auxPedidos.Cabecera.CCO_DIA
            //                     && v.CCO_MES == auxPedidos.Cabecera.CCO_MES
            //                     && v.AGE_CODIGO == auxPedidos.Cabecera.CCO_AGENTE
            //                     select new
            //                     {
            //                         cc.CCO_NUMERO,
            //                         cc.CCO_FECHA,
            //                         ct.CTI_NOMBRE,
            //                         cc.CCO_DETALLE,
            //                         cc.CCO_CODIGO,
            //                         cc.CCO_DIA,
            //                         cc.CCO_PERIODO,
            //                         cc.CCO_CIE_COMPROBA,
            //                         cc.CCO_AGENTE,
            //                         cc.CCO_MES,
            //                         DFAC_CANTIDAD = v.DFAC_CANTIDAD,
            //                         DFAC_TOTAL= v.TOTAL_CON_DESCUENTOS,
            //                         DFAC_SECUENCIA = v.DFAC_SECUENCIA,
            //                         v.PRO_NOMBRE, v.TOTAL_KILOS
            //                     }).ToListAsync(); 


            if (result == null || !result.Any())
            {

                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            var cabecera = new AuxCComproba
            {
                CCO_NUMERO = result.First().CCO_NUMERO,
                CCO_FECHA = result.First().CCO_FECHA,
                CCO_DETALLE = result.First().CCO_DETALLE,
                DFAC_CANTIDADT = result.Sum(x => x.DFAC_CANTIDAD),
                DFAC_TOTALT = result.Sum(x => x.DFAC_TOTAL),

                CCO_EMPRESA = p_empresa
            };

            var detalles = result.Select(r => new AuxDFactura
            {
                DFAC_CANTIDAD = r.DFAC_CANTIDAD,
                DFAC_TOTAL = r.DFAC_TOTAL,
                PRO_NOMBRE = r.PRO_NOMBRE,
                DFAC_SECUENCIA = r.DFAC_SECUENCIA ,

            }).OrderBy(d =>d.DFAC_SECUENCIA).ToList();

            var pedido = new AuxPedido
            {
                Cabecera = cabecera,
                Detalles = detalles,
                //TOTAL_KILOS = result.Sum(x => x.TOTAL_KILOS)
            };


            response.Data = pedido;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            // Log the exception details (ex) here as needed
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> CreatePedidoAsync(AuxNuevoPedido auxNuevoPedidos)
    {
        var response = new ServiceResponse<object>();
        var valiped = new ServiceResponse<object>();
        valiped = await ValidaPedExisteAsync(auxNuevoPedidos);
        if (valiped.Success)
        {
            return new ServiceResponse<object>
            {
                Data = valiped.Data,
                Message = valiped.Message,
                Success = true,
                Status = 700 // sirve para indicar que el pedido ya existe
            };
        }



        if (!await ValidaCliente(auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO))
        {
            response = new ServiceResponse<object>
            {
                Data = null,
                Message = "CLIENTE BLOQUEADO",
                Success = true
            };
            return response;
        };

        var horario = await _ruteroService.ValidaHoraPedidoAsync(auxNuevoPedidos.Ccomprobai.CCO_AGENTE ?? 0, auxNuevoPedidos.Ccomprobai.CCO_FECHA);
        if (horario.Data != null)
        {
            if (horario.Success == false)
            {
                response.Success = true;
                response.Data = null;
                response.Message = horario.Message;
                return response;
            }
        }

        List<DFacturai> listadfactura = new List<DFacturai>();

        DateTime ayer = DateTime.Now.AddDays(-1);
        if (auxNuevoPedidos.Ccomprobai.CCO_BODEGA == 0 || auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO == 0 //|| auxNuevoPedidos.Ccomfaci.CTI_NOMBRE == null
            || auxNuevoPedidos.Ccomprobai.CCO_DETALLE == null || auxNuevoPedidos.Ccomprobai.CCO_AGENTE == 0 || auxNuevoPedidos.Ccomprobai.CCO_FECHA < ayer)
        {
            response.Data = null;
            response.Success = true;
            var errorMessages = new List<string>();

            // Verifica cada condición y agrega mensajes específicos para cada fallo
            if (auxNuevoPedidos.Ccomprobai.CCO_BODEGA == 0)
            {
                errorMessages.Add("CCO_BODEGA no puede ser 0.");
            }
            if (auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO == 0)
            {
                errorMessages.Add("CLI_CODIGO no puede ser 0.");
            }
            //if (auxPedidos.Cabecera.CTI_NOMBRE == null)
            //{
            //    errorMessages.Add("CTI_NOMBRE no puede ser nulo.");
            //}
            if (auxNuevoPedidos.Ccomprobai.CCO_DETALLE == null)
            {
                errorMessages.Add("CCO_DETALLE no puede ser nulo.");
            }
            //if (auxNuevoPedidos.Ccomprobai.CCO_AGENTE == 0)
            //{
            //    errorMessages.Add("CCO_AGENTE no puede ser 0.");
            //}
            if (auxNuevoPedidos.Ccomprobai.CCO_FECHA < ayer)
            {
                errorMessages.Add("CCO_FECHA no puede ser una fecha pasada.");
            }

            // Si hay errores, establece el mensaje de error con los campos fallidos
            if (errorMessages.Any())
            {
                response.Message = "Existieron problemas en los siguientes campos: " + string.Join(", ", errorMessages);
            }
            return response;
        }


        int periodo = DateTime.Now.Year;
        int mes = DateTime.Now.Month;
        int dia = DateTime.Now.Day;
        DateTime _fecha = DateTime.Now;
        DateTime fecha = auxNuevoPedidos.Ccomprobai.CCO_FECHA;
        decimal cco_codigo = 0;

        int e = p_empresa;


        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT ccomprobai_s_codigo.NEXTVAL FROM dual";
                    await _context.Database.OpenConnectionAsync();

                    var result = await command.ExecuteScalarAsync();
                    cco_codigo = Convert.ToDecimal(result);
                }
                var _almacen = await _context.BODEGA.Where(b => b.BOD_CODIGO == auxNuevoPedidos.Ccomprobai.CCO_BODEGA).ToListAsync();
                int almacen = _almacen.Select(a => a.BOD_ALMACEN).FirstOrDefault() ?? 0;

                var _numero = await _context.DTIPOCOM.Where(d => d.DTI_CTI_CODIGO == p_ped_sigla
                                                            && d.DTI_PERIODO == periodo && d.DTI_ALMACEN == almacen && d.DTI_SERIE == p_ped_serie).ToListAsync();
                var numero = _numero.Select(n => n.DTI_NUMERO).FirstOrDefault();

                var _clientes = await _context.CLIENTE.Where(c => c.CLI_CODIGO == auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO).ToListAsync();
                var _cliente = _clientes.FirstOrDefault();
                decimal lprecio = _clientes.Select(l =>l.CLI_LISTAPRE).FirstOrDefault() ?? 0;
                var _impuesto_v = await _context.SISTEMA.Where(i => i.SIS_CODIGO == 1).ToListAsync();
                var _imp_porcentaje = await _context.IMPUESTO.Where(i => i.IMP_CODIGO == _impuesto_v.Select(z => z.SIS_IMPUESTO_VENTA).FirstOrDefault()).ToListAsync();

                var _politicas = await _context.POLITICA.Where(p => p.POL_CODIGO == _cliente.CLI_POLITICAS).ToListAsync(); 
                var _politica = _politicas.FirstOrDefault();
                var ccomprobai = new CComprobai
                {
                    CCO_EMPRESA = p_empresa,
                    CCO_CODIGO = cco_codigo,
                    CCO_PERIODO = periodo,
                    CCO_ALMACEN = almacen,
                    CCO_SERIE = p_ped_serie,
                    CCO_NUMERO = numero,
                    CCO_DOCTRAN = p_ped_doctran,
                    CCO_TIPODOC = p_ped_tipodoc,
                    CCO_FECHA = _fecha.Date,
                    CCO_DETALLE = auxNuevoPedidos.Ccomprobai.CCO_DETALLE ?? p_ped_doctran,
                    CCO_MODULO = p_ped_modulo,
                    CCO_NOCONTABLE = p_ped_notcontable,
                    CCO_ESTADO = p_ped_estado,
                    CCO_DESCUADRE = p_ped_descuadre,
                    CCO_ADESTINO = almacen,
                    CCO_PVENTA = p_ped_serie,
                    CCO_CENTRO = p_ped_centro,
                    CCO_TIPO_CAMBIO = p_ped_tipocambio,
                    CCO_TCLIPRO = p_ped_tclipro,
                    CCO_CODCLIPRO = auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO,
                    CCO_AGENTE = auxNuevoPedidos.Ccomprobai.CCO_AGENTE ??0,
                    CCO_TRANSACC = p_ped_trasacc,
                    CCO_ANULADO = p_ped_anulado,
                    CCO_BODEGA = auxNuevoPedidos.Ccomprobai.CCO_BODEGA,
                    CCO_DIA = dia,
                    CCO_MES = mes,
                    CCO_ANIO = periodo,
                    CCO_CONCEPTO = auxNuevoPedidos.Ccomprobai.CCO_DETALLE ?? p_ped_doctran,
                    CCO_SIGLA = p_ped_sigla
                };
                await _context.CCOMPROBAI.AddAsync(ccomprobai);
                int ccomprobaisave = await _context.SaveChangesAsync();

                if (ccomprobaisave != 0)
                {
                    var ccomfaci = new Ccomfaci
                    {
                        CFAC_EMPRESA = p_empresa,
                        CFAC_CCO_COMPROBA = ccomprobai.CCO_CODIGO,
                        CFAC_AUTORIZA = p_ped_ccomfa_autoriza,
                        CFAC_POLITICA = _clientes.Select(c => c.CLI_POLITICAS).FirstOrDefault() ?? 0,
                        CFAC_LISTA_PRECIOS = _clientes.Select(c => c.CLI_LISTAPRE).FirstOrDefault() ?? 0,
                        CFAC_EST_ENTREGA = p_ped_est_entrega,
                        CFAC_PROC_FAC = p_ped_proc_fac,
                        CFAC_PROCESO = p_ped_cfac_proceso,
                        CFAC_NOMBRE = _clientes.Select(c => c.CLI_NOMBRE).First() ?? "",
                        CFAC_DIRECCION = _clientes.Select(c => c.CLI_DIRECCION).First() ?? "",
                        CFAC_TELEFONO = _clientes.Select(c => c.CLI_TELEFONO1).First() ?? "",
                        CFAC_CED_RUC = _clientes.Select(c => c.CLI_RUC_CEDULA).First() ?? "",
                        CFAC_CIUDAD = _clientes.Select(c => c.CLI_CIUDAD).FirstOrDefault() ?? 0,
                        CFAC_TIPO_ACTPRO = p_ped_tipo_actpro,
                        CFAC_SOL_COMPROBA = p_ped_sol_comproba,
                        CFAC_IMPUESTO = _impuesto_v.Select(i => (int?)i.SIS_IMPUESTO_VENTA).FirstOrDefault() ?? 0,
                        CFAC_PORC_IMPUESTO = _imp_porcentaje.Select(i => i.IMP_PORCENTAJE).FirstOrDefault(),
                        CFAC_FECHA_FAC = auxNuevoPedidos.Ccomfaci?.CFAC_FECHA_FAC?.Date ?? _fecha.Date,
                        CFAC_TIPOPAGO = p_ped_tipopago,
                        CFAC_COMISION = p_ped_comision,
                        CFAC_IMPRIMIO = p_ped_imprimio, 
                        //CFAC_ORDEN = null, //p_ped_cfac_orden,
                        //CFAC_PEDIDO =  p_ped_cfac_orden

                    };
                    await _context.CCOMFACI.AddAsync(ccomfaci);
                    int ccomfacisave = await _context.SaveChangesAsync();
                    if (ccomfacisave != 0)
                    {
                        foreach (var auxint in auxNuevoPedidos.DFacturai)
                        {
                            var _response = (await _productoService.GetDescuentosxProductoAsync(auxint.DFAC_PRODUCTO ?? 0, lprecio, ccomprobai.CCO_CODCLIPRO)).Data as List<DListadsc>;
                            var descuento = _response?.FirstOrDefault() as DListadsc;
                            if (descuento?.DLD_PORCENTAJE == null || descuento == null)
                            {
                                descuento ??= new DListadsc(); // Inicializa descuento si es null
                                descuento.DLD_PORCENTAJE = 0;
                            };

                            var dfaturai = new DFacturai
                            {
                                DFAC_EMPRESA = p_empresa,
                                DFAC_CFAC_COMPROBA = ccomprobai.CCO_CODIGO,
                                DFAC_SECUENCIA = auxint.DFAC_SECUENCIA,
                                DFAC_PRODUCTO = auxint.DFAC_PRODUCTO,
                                DFAC_CATPRODUCTO = p_pedd_catproducto,
                                DFAC_CANTIDAD = auxint.DFAC_CANTIDAD,
                                DFAC_CANAPR = p_pedd_canapr,
                                DFAC_PRECIO = await GetPrecioAsync(lprecio, auxint.DFAC_PRODUCTO ?? 0),
                                DFAC_DESCUENTO = descuento.DLD_PORCENTAJE,
                                DFAC_BODEGA = auxint.DFAC_BODEGA,
                                DFAC_TOTAL = await GetTotalPrecioAsync(lprecio, auxint.DFAC_PRODUCTO ?? 0, auxint.DFAC_CANTIDAD),
                                DFAC_CANENT = auxint.DFAC_CANENT,
                                DFAC_CANDEV = p_pedd_candev,
                                //DFAC_CANRES = auxint.DFAC_CANRES,
                                DFAC_DSCITEM = p_pedd_dscitem,
                                DFAC_COMBO = p_pedd_combo,
                                DFAC_IVAITEM = p_pedd_ivaitem,
                                DFAC_GRABAIVA = auxint.DFAC_GRABAIVA,
                                DFAC_UDIGITADA = auxint.DFAC_UDIGITADA,
                                DFAC_CDIGITADA = auxint.DFAC_CANTIDAD,
                                
                                DFAC_CEQ = null,
                                DFAC_UEQ = null,
                                DFAC_CANT_PEDIDA = auxint.DFAC_CANT_PEDIDA,
                                DFAC_PROMOCION = auxint.DFAC_PROMOCION,
                                DFAC_ESTADO = p_pedd_estado,
                                DFAC_CAPRDIGITADA = null
                            };
                            listadfactura.Add(dfaturai);
                        }
                        await _context.DFACTURAI.AddRangeAsync(listadfactura);
                        int dfacturaisave = await _context.SaveChangesAsync();
                        if (dfacturaisave != 0)
                        {
                            var totali = new Totali
                            {
                                TOT_EMPRESA = p_empresa,
                                TOT_CCO_COMPROBA = ccomprobai.CCO_CODIGO,
                                TOT_IMPUESTO = p_pedd_totimpuesto,
                                TOT_PORC_DESC = _politica?.POL_PORC_DESC,
                                TOT_PORC_FINANC = _politica?.POL_PORC_FINANC,
                                TOT_PORC_PRO_PAGO = _politica?.POL_PORC_PRO_PAGO,
                                TOT_PORC_PAG_CONTA = _politica?.POL_PORC_PAG_CONTA,
                                TOT_LINEA_CREDITO = _politica?.POL_LINEA_CREDITO,
                                TOT_DIAS_PLAZO = _politica?.POL_DIAS_PLAZO,
                                TOT_NRO_PAGOS = _politica?.POL_NRO_PAGOS,
                                TOT_SUBTOTAL = 0,
                                TOT_DESCUENTO1 = 0,
                                TOT_DESCUENTO2 = 0,
                                TOT_TIMPUESTO = 0,
                                TOT_TRANSPORTE = 0,
                                TOT_SEGURO_TRANS = 0,
                                TOT_AJUSTE = 0,
                                TOT_FINANCIA = 0,
                                TOT_TOTAL = 0,
                                TOT_PORC_IMPUESTO = 12,
                                TOT_DESC1_0 = 0,
                                TOT_DESC2_0 =0,
                                TOT_SUBTOT_0 = 0
                            };
                            await _context.TOTALI.AddAsync(totali);
                            int totalisave = await _context.SaveChangesAsync();
                            if (totalisave != 0)
                            await _ruteroService.SetRuteroPedidoAsync(ccomprobai.CCO_CODCLIPRO, ccomprobai.CCO_AGENTE ?? 0, ccomprobai.CCO_FECHA, _cliente.CLI_ZONA ?? 0); /// registra en el rutero visita y pedido
                            string agente = await _agenteService.GetUsuarioAsync(ccomprobai.CCO_AGENTE ?? 0);
                            if (agente != null)
                            {
                                ccomprobai.CREA_USR = agente;
                                _context.CCOMPROBAI.Update(ccomprobai);
                                await _context.SaveChangesAsync();

                            }
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            transaction.Rollback();
                            response.Data = null;
                            response.Success = false;
                            response.Message = "Existió un problema por favor vuelva a intentarlo.";
                            return response;

                        }
                        //await transaction.CommitAsync();
                        
                        response.Data = new { ccomprobai, listadfactura };
                        response.Success = true;
                        response.Message = "Pedido guardado Existosamente # de pedido = " + ccomprobai.CCO_CODIGO;
                        return response;
                    }
                    else
                    {
                        transaction.Rollback();
                        response.Data = null;
                        response.Success = false;
                        response.Message = "Existió un problema por favor vuelva a intentarlo.";
                        return response;
                    }
                }

                response.Data = null;
                response.Success = false;
                response.Message = "Existió un problema por favor vuelva a intentarlo.";
                return response;
            }
        }
        catch (Exception ex)
        {
          
            response.Data = auxNuevoPedidos;
            response.Success = false;
            response.Message = "Existió un problema por favor vuelva a intentarlo." + ex.ToString();
            return response;
        }

    }

   

    public async Task<decimal> GetPrecioAsync(decimal lprecio, decimal cproducto)
    {
        var fechaActual = DateTime.Today;
        DateTime d = fechaActual.Date;

        var _pprecio = await _context.DLISTAPRE.Where(d => d.DLP_LISTAPRE == lprecio
                           && d.DLP_PRODUCTO == cproducto
                           && d.DLP_FECHA_INI <= fechaActual
                           && (d.DLP_FECHA_FIN == null || d.DLP_FECHA_FIN >= fechaActual)
                           && (d.DLP_INACTIVO ?? 0) == 0)
                           .Select(d => d.DLP_PRECIO).ToListAsync();


        decimal pprecio = _pprecio.FirstOrDefault();

        if (pprecio > 0.01m)
        {
            return Math.Round(pprecio, 2);
        }
        else
            return pprecio;
        
    }

    public async Task<decimal> GetTotalPrecioAsync(decimal lprecio, decimal cproducto, decimal cdigitada)
    {
        var fechaActual = DateTime.Today;
        DateTime d = fechaActual.Date;

        decimal total = 0;
        var _pprecio = await _context.DLISTAPRE.Where(d => d.DLP_LISTAPRE == lprecio 
                            && d.DLP_PRODUCTO == cproducto 
                            && d.DLP_FECHA_INI <= fechaActual
                            && (d.DLP_FECHA_FIN == null || d.DLP_FECHA_FIN >= fechaActual)
                            &&  (d.DLP_INACTIVO ?? 0) == 0)
            .Select(d => d.DLP_PRECIO)
            //.DefaultIfEmpty(0)
            .ToListAsync();
        decimal pprecio = _pprecio.FirstOrDefault();
        if (pprecio != 0 && cdigitada !=0)
        {
             total = (pprecio * cdigitada);
        }
        else { total = 0; }
        return Math.Round(total,2);
    }


    public async Task<bool> ValidaCliente(decimal cliente)
    {


        var bloqueado = await (from c in _context.CLIENTE
                      where c.CLI_CODIGO == cliente
                            && ((c.CLI_BLOQUEO ?? 0) == 1 || (c.CLI_INACTIVO ?? 0) == 1)
                      select c).CountAsync();

        int resultado = bloqueado > 0 ? bloqueado : 0;
        if (resultado > 0)
        {
            return false;
        }
        return true ;

    }


    public async Task<ServiceResponse<object>> GetPedidosDiaxAgente(decimal agente)
    {
        var response = new ServiceResponse<object>();
        try
        {

            var today = DateTime.Today.Date;

            var pedidos = await _context.REP_CANTIDADES_PEDIDOSA
                .Where(v => v.CCO_FECHA >= today && v.AGE_CODIGO == agente)
                .GroupBy(v => new { v.DOC, v.CLI_NOMBRE, v.CCO_FECHA, v.AGE_CODIGO, v.FACTURA })
                .Select(g => new
                {
                    DOC = g.Key.DOC,
                    CLI_NOMBRE = g.Key.CLI_NOMBRE,
                    CCO_FECHA = g.Key.CCO_FECHA,
                    KILOS = g.Sum(v => v.TOTAL_KILOS),
                    TOTAL = g.Sum(v => v.TOTAL_CON_DESCUENTOS),
                    AGE_CODIGO = g.Key.AGE_CODIGO,
                    FACTURA = g.Key.FACTURA,
                    CREA_FECHA = g.Max(v => v.CREA_FECHA)
                }).OrderBy(o =>o.CREA_FECHA)
                .ToListAsync();
            if (pedidos == null || !pedidos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = Utils.FormatosTexto.DatosNoEncontrados; ;
            }
            else
            {
                response.Data = pedidos;
                response.Success = true;
                response.Message = Utils.FormatosTexto.DatosEncontrados;
            }
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            return response;
        }

    }



    public async Task<ServiceResponse<object>> GetPedidosxDiaAsync(PedidosDiaRequest request) /// DEVUELVE LOS PEDIDO DE UN DÍA SELECCIONADO
    {
        var response = new ServiceResponse<object>();
        try
        {

            var day = request.Date?.Date;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                // Define la consulta SQL con parámetros
                command.CommandText = @"
                            SELECT DOC, CLI_NOMBRE, CCO_FECHA, SUM(TOTAL_KILOS) AS TOTAL_KILOS, 
                                SUM(TOTAL_CON_DESCUENTOS) AS TOTAL_CON_DESCUENTOS, AGE_CODIGO, FACTURA, 
                                MAX(CREA_FECHA) AS CREA_FECHA, CCO_NUMERO, CLI_CODIGO, CLI_CODIGO, CCO_PERIODO, CCO_MES, CCO_DIA, CCO_CODIGO,  CFAC_FECHA_FAC,
                            SUM(DFAC_CANT_PEDIDA) AS DFAC_CANT_PEDIDA
                            FROM REP_CANTIDADES_PEDIDOSA
                            WHERE TRUNC(CREA_FECHA) = :day
                              AND AGE_CODIGO = :agente
                            GROUP BY DOC, CLI_NOMBRE, CCO_FECHA, AGE_CODIGO, FACTURA,  CCO_NUMERO, CLI_CODIGO,  CLI_CODIGO, CCO_PERIODO, CCO_CODIGO , CCO_MES, CCO_DIA ,  CFAC_FECHA_FAC
                            ORDER BY CREA_FECHA";

                // Agrega los parámetros para evitar inyección SQL
                //var today = DateTime.Today;
                var agente = request.Agente;

                var todayParam = command.CreateParameter();
                todayParam.ParameterName = "day";
                todayParam.Value = day;
                todayParam.DbType = System.Data.DbType.Date;
                command.Parameters.Add(todayParam);

                var agenteParam = command.CreateParameter();
                agenteParam.ParameterName = "agente";
                agenteParam.Value = agente;
                agenteParam.DbType = System.Data.DbType.Decimal;
                command.Parameters.Add(agenteParam);

                // Abre la conexión si no está abierta
                if (command.Connection.State != System.Data.ConnectionState.Open)
                {
                    await command.Connection.OpenAsync();
                }

                // Ejecuta la consulta y procesa los resultados
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var pedidos = new List<object>();

                    while (await reader.ReadAsync())
                    {
                        pedidos.Add(new
                        {
                            DOC = reader["DOC"].ToString(),
                            CLI_NOMBRE = reader["CLI_NOMBRE"].ToString(),
                            CCO_FECHA = reader.GetDateTime(reader.GetOrdinal("CCO_FECHA")),
                            TOTAL_KILOS = reader.GetDecimal(reader.GetOrdinal("TOTAL_KILOS")),
                            TOTAL_CON_DESCUENTOS = reader.GetDecimal(reader.GetOrdinal("TOTAL_CON_DESCUENTOS")),
                            AGE_CODIGO = reader.GetDecimal(reader.GetOrdinal("AGE_CODIGO")),
                            FACTURA = reader["FACTURA"].ToString(),
                            CREA_FECHA = reader.GetDateTime(reader.GetOrdinal("CREA_FECHA")),
                            CCO_NUMERO = reader.GetDecimal(reader.GetOrdinal("CCO_NUMERO")),
                            CLI_CODIGO = reader.GetDecimal(reader.GetOrdinal("CLI_CODIGO")),
                            CCO_PERIODO = reader.GetInt16(reader.GetOrdinal("CCO_PERIODO")),
                            CCO_MES = reader.GetInt16(reader.GetOrdinal("CCO_MES")),
                            CCO_DIA=  reader.GetInt16(reader.GetOrdinal("CCO_DIA")), 
                            CCO_CODIGO = reader.GetDecimal(reader.GetOrdinal("CCO_CODIGO")),
                            CFAC_FECHA_FAC = reader.GetDateTime(reader.GetOrdinal("CFAC_FECHA_FAC")),
                            DFAC_CANT_PEDIDA = reader.GetDecimal(reader.GetOrdinal("DFAC_CANT_PEDIDA"))
                        });
                    }

                  if (pedidos == null || !pedidos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = Utils.FormatosTexto.DatosNoEncontrados;
                return response;
            }
            else
            {
                response.Data = pedidos;
                response.Success = true;
                response.Message = Utils.FormatosTexto.DatosEncontrados;
            }  
                }
            }
           
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            return response;
        }
    }


    public async Task<ServiceResponse<object>> GetFacsxClienteAsync(decimal request)
    {

        var response = new ServiceResponse<object>();
        try
        {
            var result = await _context.REP_CANTIDADES_VENTAS_2009
                .Where(r =>
                    r.CLI_CODIGO == request &&
                    r.CCO_FECHA >= DateTime.Today.AddDays(-60) &&
                    r.CTI_ID == "FAC")
                .GroupBy(r => new { r.DOC, r.CCO_FECHA, r.CLI_CODIGO, r.AGE_CODIGO })
                .Select(g => new
                {
                    DOC = g.Key.DOC,
                    CCO_FECHA = g.Key.CCO_FECHA,
                    CLI_CODIGO = g.Key.CLI_CODIGO,
                    AGE_CODIGO = g.Key.AGE_CODIGO,
                    TOTAL_KILOS = g.Sum(r => r.TOTAL_LIBRAS),
                    TOTAL_CON_DESCUENTOS = g.Sum(r => r.TOTAL_CON_DESCUENTOS)
                })
                .OrderByDescending(r => r.CCO_FECHA)
                .ToListAsync();

            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            response.Data = result;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            return response;
        }
    }


    public async Task<ServiceResponse<object>> GetFacDetalleAsync(Rep_Cantidades_Pedidosa request)
    {

        var response = new ServiceResponse<object>();
        try
        {
            var result = await _context.REP_CANTIDADES_VENTAS_2009
                .Where(r =>
                    r.CLI_CODIGO == request.CLI_CODIGO &&
                    r.CCO_FECHA == request.CCO_FECHA && 
                    r.AGE_CODIGO == request.AGE_CODIGO &&
                    r.DOC == request.DOC &&
                    r.CTI_ID == "FAC")
                .GroupBy(r => new { r.DOC, r.CCO_FECHA, r.CLI_CODIGO, 
                    r.PRO_NOMBRE, r.TOTAL_LIBRAS, r.TOTAL_CON_DESCUENTOS,
                    r.DFAC_SECUENCIA, r.DFAC_CDIGITADA
                })
                .Select(g => new
                {
                    DOC = g.Key.DOC,
                    CCO_FECHA = g.Key.CCO_FECHA,
                    CLI_CODIGO = g.Key.CLI_CODIGO,
                    PRO_NOMBRE = g.Key.PRO_NOMBRE,
                    TOTAL_KILOS = g.Key.TOTAL_LIBRAS,
                    TOTAL_CON_DESCUENTOS = g.Key.TOTAL_CON_DESCUENTOS,
                    DFAC_SECUENCIA = g.Key.DFAC_SECUENCIA,
                    DFAC_CDIGITADA = g.Key.DFAC_CDIGITADA
                })
                .OrderByDescending(r => r.CCO_FECHA)
                .ToListAsync();

            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            response.Data = result;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            return response;
        }
    }



    public async Task<ServiceResponse<object>> GetEnvHoyAsync(decimal agente) /// DEVUELVE LOS PEDIDO DE UN DÍA SELECCIONADO
    {
        var response = new ServiceResponse<object>();
        try
        {
                    var pedidos = await _context.REP_PEDIDOS_INT_X_DAPP.Where(p => p.CCO_AGENTE == agente).OrderBy(o =>o.CLI_NOMBRE).ToListAsync();
            
                    if (pedidos == null || !pedidos.Any())
                    {
                        response.Data = null;
                        response.Success = true;
                        response.Message = Utils.FormatosTexto.DatosNoEncontrados;
                        return response;
                    }
                    else
                    {
                        response.Data = pedidos;
                        response.Success = true;
                        response.Message = Utils.FormatosTexto.DatosEncontrados;
                        return response;
                    }

        }

        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            return response;
        }
    }



    public async Task<ServiceResponse<object>> ValidaPedExisteAsync(AuxNuevoPedido auxNuevoPedidos)
    {
        var response = new ServiceResponse<object>();
        try
        {
            DateTime _fecha = DateTime.Now;
            if (auxNuevoPedidos == null || auxNuevoPedidos.Ccomprobai == null || auxNuevoPedidos.DFacturai == null)
            {
                response.Data = null;
                response.Success = false;
                response.Message = "pedidos no encontrados.";
                return response;
            }
           // var productosSolicitados = auxNuevoPedidos.DFacturai
           //.Select(d => d.DFAC_PRODUCTO)
           //.Where(p => p.HasValue)
           //.Select(p => p.Value)
           //.OrderBy(p => p)
           //.ToList();

            var pedidosCandidatos = await _context.CCOMPROBAI
            .Where(cc =>
                cc.CCO_CODCLIPRO == auxNuevoPedidos.Ccomprobai.CCO_CODCLIPRO &&
                cc.CCO_FECHA.Date == DateTime.Now.Date &&
                cc.CCO_AGENTE == auxNuevoPedidos.Ccomprobai.CCO_AGENTE && cc.CCO_ESTADO != 9)
            .ToListAsync();

            foreach (var pedidoR in pedidosCandidatos)
            {
                // Obtener productos del pedido actual
                var productosPedido = await _context.DFACTURAI
       .Where(df => df.DFAC_CFAC_COMPROBA == pedidoR.CCO_CODIGO)
       .Where(df => df.DFAC_PRODUCTO.HasValue)
       .Select(df => new { Producto = df.DFAC_PRODUCTO.Value, Cantidad = df.DFAC_CANTIDAD })
       .OrderBy(x => x.Producto)
       .ToListAsync();


                var productosSolicitados = auxNuevoPedidos.DFacturai
                      .Where(d => d.DFAC_PRODUCTO.HasValue)
                      .Select(d => new { Producto = d.DFAC_PRODUCTO.Value, Cantidad = d.DFAC_CANTIDAD })
                      .OrderBy(x => x.Producto)
                      .ToList();

                // Compara ambas listas
                bool iguales = productosPedido.SequenceEqual(productosSolicitados);

                if (iguales)
                {
                    var pedidoRS = await (from cc in _context.CCOMPROBAI
                                         where cc.CCO_CODIGO == pedidoR.CCO_CODIGO

                                         select new CComprobai 
                                         {
                                            CCO_EMPRESA = cc.CCO_EMPRESA,
                                            CCO_CODIGO = cc.CCO_CODIGO,
                                            CCO_PERIODO = cc.CCO_PERIODO,
                                            CCO_SIGLA= cc.CCO_SIGLA,
                                             CCO_ALMACEN= cc.CCO_ALMACEN,
                                             CCO_SERIE= cc.CCO_SERIE,
                                             CCO_DOCTRAN = cc.CCO_DOCTRAN,
                                             CCO_TIPODOC= cc.CCO_TIPODOC,
                                             CCO_FECHA=  cc.CCO_FECHA,
                                             CCO_CONCEPTO=  cc.CCO_CONCEPTO,
                                             CCO_MODULO=  cc.CCO_MODULO,
                                             CCO_NOCONTABLE=   cc.CCO_NOCONTABLE,
                                             CCO_ESTADO=  cc.CCO_ESTADO,
                                             CCO_DESCUADRE=  cc.CCO_DESCUADRE,
                                             CCO_ADESTINO=  cc.CCO_ADESTINO,
                                             CCO_PVENTA=   cc.CCO_PVENTA,
                                             CCO_CENTRO=   cc.CCO_CENTRO,
                                             CCO_TIPO_CAMBIO= cc.CCO_TIPO_CAMBIO,
                                             CCO_TCLIPRO= cc.CCO_TCLIPRO,
                                             CCO_CODCLIPRO= cc.CCO_CODCLIPRO,
                                             CCO_AGENTE=  cc.CCO_AGENTE,
                                             CCO_TRANSACC=  cc.CCO_TRANSACC,
                                             CCO_ANULADO=   cc.CCO_ANULADO,
                                             CCO_BODEGA=  cc.CCO_BODEGA,
                                             CCO_DIA=  cc.CCO_DIA,
                                             CCO_MES=  cc.CCO_MES,
                                             CCO_ANIO= cc.CCO_ANIO,
                                             CCO_DETALLE= cc.CCO_DETALLE,
                                             CCO_NUMERO= cc.CCO_NUMERO

                                         }
                               ).ToListAsync();
                    var pedido = new AuxNuevoPedido
                    {
                        Ccomprobai = pedidoRS.FirstOrDefault(),
                        
                    };
                  
                    if (pedido.Ccomprobai == null)
                    {
                        response.Data = null;
                        response.Success = false;
                        response.Message = "PEDIDO NO EXISTE";
                        return response;
                    }
                    else
                    {
                        response.Data = pedido;
                        response.Success = true;
                        response.Message = "PEDIDO YA INGRESADO NÚMERO " + pedido?.Ccomprobai?.CCO_NUMERO;
                        return response;
                    }

                }
            }

            response.Data = null;
            response.Success = false;
            response.Message = "pedidos no encontrados.";
            return response;

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos";
            throw new DatabaseException("Error de base de datos.", ex);
        }
    }
}
