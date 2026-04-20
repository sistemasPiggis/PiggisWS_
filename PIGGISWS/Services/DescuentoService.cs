using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
namespace PIGGISWS.Services;



public class DescuentoService : IDescuentoService
{


    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevolucionesService> _logger;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;


    public DescuentoService(ApplicationDbContext context, ILogger<DevolucionesService> logger)
    {
        _context = context;
        _logger = logger;
        GetParametros();
    }
    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DescuentoService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetParametros() " + ex.ToString() );
            Console.WriteLine(ex.ToString());
        }

    }

    
    public async Task<ServiceResponse<object>> GetDescuentoxAgenteAsync(int agente)
    {
        var respuesta = new ServiceResponse<object>(); 
        Stopwatch cronometro = new Stopwatch();
        Stopwatch cronometroTotal = Stopwatch.StartNew();

        try
        {
            DateTime fecha = DateTime.Now;


            var bodega = await (from u in _context.USUARIO
                                join b in _context.USRBOD on u.USR_CODIGO equals b.UBO_USUARIO
                                where u.USR_AGENTE == agente
                                select Convert.ToDecimal(b.UBO_BODEGA)).ToListAsync();


            //var q1 = await (from cc in _context.CC_EST_PEDIDOS
            //                join c in _context.CLIENTE on cc.CCO_CODCLIPRO equals c.CLI_CODIGO
            //                where c.CLI_AGENTE == agente
            //                select cc.PRO_CODIGO
            //                ).ToListAsync();

            //var q2 = await (from vl in _context.VL_CC_EST_PEDIDOSQ
            //                join cl in _context.CLIENTE on vl.CCO_CODCLIPRO equals cl.CLI_CODIGO
            //                where cl.CLI_AGENTE == agente
            //                   && cl.CLI_EMPRESA == p_empresa
            //                select vl.PRO_CODIGO).ToListAsync();


            //var q3 = await (from rp in _context.REP_LISTA_PROD_PED_INTERNET
            //                join p in _context.PRODUCTO on rp.PRO_CODIGO equals p.PRO_CODIGO
            //                where p.PRO_BODEGA != null && bodega.Contains((decimal)p.PRO_BODEGA)
            //                select Convert.ToDecimal(p.PRO_CODIGO)).ToListAsync();

            //var productospedidos = q1.AsEnumerable()
            //    .Union(q2.AsEnumerable())
            //    .Union(q3.AsEnumerable())
            //    .Distinct()
            //    .ToList();

            var desc = await (from cl in _context.CLIENTE
                              join ls in _context.DLISTADSC
                                on new { Lista = cl.CLI_LISTAPRE, Empresa = cl.CLI_EMPRESA }
                                equals new { Lista = (decimal?)ls.DLD_LISTAPRE, Empresa = ls.DLD_EMPRESA }
                              where cl.CLI_EMPRESA == p_empresa
                             && cl.CLI_AGENTE == agente
                             && ls.DLD_CLIENTE == null
                             && cl.CLI_INACTIVO == 0
                             && ((ls.DLD_FECHA_INI <= fecha && ls.DLD_FECHA_FIN >= fecha) || ls.DLD_FECHA_FIN == null)
                             && ls.DLD_PRODUCTO != null //&& productospedidos.Contains((decimal)ls.DLD_PRODUCTO)

                             && (ls.DLD_INACTIVO == 0 || ls.DLD_INACTIVO == null)


                              select new
                              {
                                  cl.CLI_EMPRESA,
                                  cl.CLI_LISTAPRE,
                                  ls.DLD_CODIGO,
                                  cl.CLI_CODIGO,
                                  ls.DLD_CLIENTE,
                                  ls.DLD_PRODUCTO,
                                  ls.DLD_UMEDIDA,
                                  DLD_PORCENTAJE = ls.DLD_PORCENTAJE ?? 0M,
                                  DLD_CADACUANTOS = ls.DLD_CADACUANTOS ?? 0M,
                                  DLD_CUANTOS = ls.DLD_CUANTOS ?? 0,
                                  DLD_FISICO = ls.DLD_FISICO ?? 0
                              }).Distinct().ToListAsync();


            var cadacuantos = await (from cl in _context.CLIENTE
                                     join ls in _context.DLISTADSC
                                     on new { Lista = cl.CLI_LISTAPRE, Empresa = cl.CLI_EMPRESA }
                                     equals new { Lista = (decimal?)ls.DLD_LISTAPRE, Empresa = ls.DLD_EMPRESA }

                                     where cl.CLI_EMPRESA == p_empresa
                                    && cl.CLI_AGENTE == agente
                                    && cl.CLI_INACTIVO == 0
                                    && ls.DLD_CLIENTE == cl.CLI_CODIGO
                                    && ((ls.DLD_FECHA_INI <= fecha && ls.DLD_FECHA_FIN >= fecha) 
                                    || ls.DLD_FECHA_FIN == null)
                                    && (ls.DLD_INACTIVO == 0 || ls.DLD_INACTIVO == null)
                                     select new
                                     {
                                         cl.CLI_EMPRESA,
                                         cl.CLI_LISTAPRE,
                                         ls.DLD_CODIGO,
                                         cl.CLI_CODIGO,
                                         ls.DLD_CLIENTE,
                                         ls.DLD_PRODUCTO,
                                         ls.DLD_UMEDIDA,
                                         DLD_PORCENTAJE = ls.DLD_PORCENTAJE ?? 0M,
                                         DLD_CADACUANTOS = ls.DLD_CADACUANTOS ?? 0M,
                                         DLD_CUANTOS = ls.DLD_CUANTOS ?? 0,
                                         DLD_FISICO = ls.DLD_FISICO ?? 0
                                     }).Distinct().ToListAsync();

            var descuentos = desc.AsEnumerable()
                .Union(cadacuantos.AsEnumerable())
                .ToList();


            respuesta.Success = true;
            respuesta.Data = descuentos;
            respuesta.Message = "SE RECUPERO LISTA DE DESCUENTOS";
            respuesta.Status = 200;

            cronometroTotal.Stop();
            _logger.LogInformation($"------------------XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX----------------[GetDescuentoxAgenteAsync TOTAL] Todo el bloque de Descuentos tardó: {cronometroTotal.ElapsedMilliseconds} ms + {descuentos.Count}");
            return respuesta;

        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = "Error al obtener el descuento";
            respuesta.Status = 500;
            _logger.LogError(" --------------------- ERROR ------------------ GetDescuentoxAgenteAsync() " + ex.ToString());
            return respuesta;
        }
    }
}
