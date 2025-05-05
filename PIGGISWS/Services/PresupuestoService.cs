using Azure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using System.Collections.Generic;
using static Google.Apis.Requests.BatchRequest;

namespace PIGGISWS.Services;

public class PresupuestoService : IPresupuestoService
{

    private readonly ApplicationDbContext _context;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;
    int p_dias_anticipo;

    public PresupuestoService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();
    }
    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "PresupuestoService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }


    public async Task<ServiceResponse<object>> GetPresAgePeriodoAsync(AuxPresupuesto presupuesto)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var presBaja = await GetPresBajxperiodoAsync(presupuesto);
            
            var presMedia = await GetPresMedxperiodoAsync(presupuesto);
            var presAlta = await GetPresAltxperiodoAsync(presupuesto);
            var detalles = new List<PresupuestoDetalle>();

            if (presBaja.Data is IEnumerable<dynamic> bajaData)
            {
                detalles.AddRange(bajaData.Select(x => new PresupuestoDetalle
                {
                    Tipo = "LINEA ECONOMICA",
                    Kilos = (decimal)x.KILOS_BAJA,
                    Presupuesto = (decimal)x.PRESUPUESTO_BAJA,
                    Cumplimiento = (decimal)x.CUMPLIMIENTO_BAJA
                }));
            }

           

            if (presMedia.Data is IEnumerable<dynamic> mediaData)
            {
                detalles.AddRange(mediaData.Select(x => new PresupuestoDetalle
                {
                    Tipo = "LINEA MEDIA",
                    Kilos = (decimal)x.KILOS_MEDIA,
                    Presupuesto = (decimal)x.PRESUPUESTO_MEDIA,
                    Cumplimiento = (decimal)x.CUMPLIMIENTO_MEDIA
                }));
            }
            if (presAlta.Data is IEnumerable<dynamic> altaData)
            {
                detalles.AddRange(altaData.Select(x => new PresupuestoDetalle
                {
                    Tipo = "LINEA ALTA",
                    Kilos = (decimal)x.KILOS_ALTA,
                    Presupuesto = (decimal)x.PRESUPUESTO_BAJA,
                    Cumplimiento = (decimal)x.CUMPLIMIENTO_BAJA
                }));
            }

            // Calcular totales
            var totalKilos = detalles.Sum(x => x.Kilos);
            var totalPresupuesto = detalles.Sum(x => x.Presupuesto);
            var totalCumplimiento = totalKilos == 0 ? 0 : Math.Round((totalKilos / totalPresupuesto) * 100, 2);

            // Preparar la respuesta
            var resultado = new
            {
                Agente = presupuesto.AGENTE,
                Mes = presupuesto.MES,
                Periodo = presupuesto.PERIODO,
                Detalles = detalles,
                TotalFacturado = totalKilos,
                TotalPresupuesto = totalPresupuesto,
                TotalCumplimiento = totalCumplimiento
            };

            if (resultado.Detalles.Count() == 0)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL MES SELECCIONADO";
                return response;
            }
            response.Data = resultado;
            response.Success = true;
            response.Message = "Datos obtenidos exitosamente.";
            return response;
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = $"Error al obtener los datos: {ex.Message}";
            respuesta.Data = null;
            return respuesta;
        }
    }


    public async Task<ServiceResponse<object>> GetPresBajxperiodoAsync(AuxPresupuesto presupuesto) // Presupuesto por periodo, mes y agente
    {
        var response = new ServiceResponse<object>();
        int factorpres = 1;
        bool band = false;
        if (presupuesto.DIA == null || presupuesto.DIA == 0 || presupuesto.DIA == -1)
        {
            band = true;
        }
        try
        {
         
                var datosBase = (band)
                   ? await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                            where rcp.PERIODO == presupuesto.PERIODO
                                  && rcp.MES == presupuesto.MES
                                  && rcp.AGE_CODIGO == presupuesto.AGENTE
                                  && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                  && rcp.CLASIFICACION == "ECONOMICA"
                            group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                            select new
                            {
                                grupo.Key.AGE_CODIGO,
                                grupo.Key.PERIODO,
                                grupo.Key.MES,
                                KILOS_BAJA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                            }).ToListAsync()
                   : await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                            where rcp.PERIODO == presupuesto.PERIODO
                                  && rcp.MES == presupuesto.MES
                                   && rcp.CCO_FECHA >= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIA ?? 1)
                                   && rcp.CCO_FECHA <= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIAF ?? 1)
                                  && rcp.AGE_CODIGO == presupuesto.AGENTE
                                  && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                  && rcp.CLASIFICACION == "ECONOMICA"
                            group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                            select new
                            {
                                grupo.Key.AGE_CODIGO,
                                grupo.Key.PERIODO,
                                grupo.Key.MES,
                                KILOS_BAJA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                            }).ToListAsync();
            
          
                var presupuestos = await (from p in _context.PRE_VENTAS_ANUAL
                                      where p.PVA_CALIDAD_TX == "BAJA"
                                      select new
                                      {
                                          p.PVA_COD_AGENTE_FK,
                                          p.PVA_PERIODO_NR,
                                          p.PVA_MES_NR,
                                          PVA_VALOR_NR = p.PVA_VALOR_NR ?? 0
                                      }).ToListAsync();

            var resultado = datosBase.Select(dato =>
            {
                var presupuestoBaja = presupuestos
                    .FirstOrDefault(p => p.PVA_COD_AGENTE_FK == dato.AGE_CODIGO
                                         && p.PVA_PERIODO_NR == dato.PERIODO
                                         && p.PVA_MES_NR == dato.MES)?.PVA_VALOR_NR ?? 0;

                if (presupuesto.DIA != null && presupuesto.DIA != 0 && presupuesto.DIA != -1)
                {
                    int diasmes = DateTime.DaysInMonth(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0);
                    int dias = (presupuesto.DIAF ?? 0) - (presupuesto.DIA-1 ?? 0);
                    if (dias > 0)
                    {
                        presupuestoBaja = (presupuestoBaja / diasmes) * dias;
                    }
                    else
                    {
                        presupuestoBaja = (presupuestoBaja / diasmes);
                    }
                   
                }

                var cumplimientoBaja = (presupuestoBaja) == 0
                    ? 0
                    : Math.Round((dato.KILOS_BAJA / (presupuestoBaja)) * 100, 2);

                return new
                {
                   
                    dato.KILOS_BAJA,
                    PRESUPUESTO_BAJA = Math.Round((presupuestoBaja), 2),
                    CUMPLIMIENTO_BAJA = cumplimientoBaja
                };
            }).ToList();

            // Paso 4: Preparar la respuesta
            response.Data = resultado;
            response.Success = true;
            response.Message = "Datos obtenidos exitosamente.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener los datos: {ex.Message}";
            response.Data = null;
        }

        return response;
    }


    public async Task<ServiceResponse<object>> GetPresAltxperiodoAsync(AuxPresupuesto presupuesto)
    {
        var response = new ServiceResponse<object>();
        int factorpres = 1;
        try
        {

            var datosBase = (presupuesto.DIA == null || presupuesto.DIA == 0 || presupuesto.DIA == -1)
                    ? await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                             where rcp.PERIODO == presupuesto.PERIODO
                                   && rcp.MES == presupuesto.MES
                                   && rcp.AGE_CODIGO == presupuesto.AGENTE
                                   && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                   && rcp.CLASIFICACION == "ALTA"
                             group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                             select new
                             {
                                 grupo.Key.AGE_CODIGO,
                                 grupo.Key.PERIODO,
                                 grupo.Key.MES,
                                 KILOS_ALTA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                             }).ToListAsync()
                    : await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                             where rcp.PERIODO == presupuesto.PERIODO
                                   && rcp.MES == presupuesto.MES
                                    && rcp.CCO_FECHA >= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIA ?? 1)
                                   && rcp.CCO_FECHA <= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIAF ?? 1)
                                   && rcp.AGE_CODIGO == presupuesto.AGENTE
                                   && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                   && rcp.CLASIFICACION == "ALTA"
                             group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                             select new
                             {
                                 grupo.Key.AGE_CODIGO,
                                 grupo.Key.PERIODO,
                                 grupo.Key.MES,
                                 KILOS_ALTA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                             }).ToListAsync();

            if (presupuesto.DIA != null && presupuesto.DIA != 0 && presupuesto.DIA != -1)
            {
                factorpres = DateTime.DaysInMonth(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0);
            }

            var presupuestos = await (from p in _context.PRE_VENTAS_ANUAL
                                      where p.PVA_CALIDAD_TX == "ALTA"
                                      select new
                                      {
                                          p.PVA_COD_AGENTE_FK,
                                          p.PVA_PERIODO_NR,
                                          p.PVA_MES_NR,
                                          PVA_VALOR_NR = p.PVA_VALOR_NR ?? 0
                                      }).ToListAsync();

            var resultado = datosBase.Select(dato =>
            {
                var presupuestoAlta = presupuestos
                    .FirstOrDefault(p => p.PVA_COD_AGENTE_FK == dato.AGE_CODIGO
                                         && p.PVA_PERIODO_NR == dato.PERIODO
                                         && p.PVA_MES_NR == dato.MES)?.PVA_VALOR_NR ?? 0;


                if (presupuesto.DIA != null && presupuesto.DIA != 0 && presupuesto.DIA != -1)
                {
                    int diasmes = DateTime.DaysInMonth(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0);
                    int dias = (presupuesto.DIAF ?? 0) - (presupuesto.DIA - 1 ?? 0);
                    if (dias > 0)
                    {
                        presupuestoAlta = (presupuestoAlta / diasmes) * dias;
                    }
                    else
                    {
                        presupuestoAlta = (presupuestoAlta / diasmes);
                    }

                }


                var cumplimientoAlta = presupuestoAlta == 0
                    ? 0
                    : Math.Round((dato.KILOS_ALTA / presupuestoAlta) * 100, 2);

                return new
                {
                    
                    dato.KILOS_ALTA,
                    PRESUPUESTO_BAJA = Math.Round(presupuestoAlta, 2),
                    CUMPLIMIENTO_BAJA = cumplimientoAlta
                };
            }).ToList();

            // Paso 4: Preparar la respuesta
            response.Data = resultado;
            response.Success = true;
            response.Message = "Datos obtenidos exitosamente.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener los datos: {ex.Message}";
            response.Data = null;
        }

        return response;
    }


    public async Task<ServiceResponse<object>> GetPresMedxperiodoAsync(AuxPresupuesto presupuesto) // Presupuesto por periodo, mes y agente
    {
        var response = new ServiceResponse<object>();
        try
        {

            var datosBase = (presupuesto.DIA == null || presupuesto.DIA == 0 || presupuesto.DIA == -1)
                   ? await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                            where rcp.PERIODO == presupuesto.PERIODO
                                  && rcp.MES == presupuesto.MES
                                  && rcp.AGE_CODIGO == presupuesto.AGENTE
                                  && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                  && rcp.CLASIFICACION == "MEDIA"
                            group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                            select new
                            {
                                grupo.Key.AGE_CODIGO,
                                grupo.Key.PERIODO,
                                grupo.Key.MES,
                                KILOS_MEDIA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                            }).ToListAsync()
                   : await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                            where rcp.PERIODO == presupuesto.PERIODO
                                  && rcp.MES == presupuesto.MES
                                    && rcp.CCO_FECHA >= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIA ?? 1)
                                   && rcp.CCO_FECHA <= new DateTime(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0, presupuesto.DIAF ?? 1)
                                  && rcp.AGE_CODIGO == presupuesto.AGENTE
                                  && !new[] { "CUE0018820", "CUE0030903" }.Contains(rcp.CLI_ID)
                                  && rcp.CLASIFICACION == "MEDIA"
                            group rcp by new { rcp.AGE_CODIGO, rcp.PERIODO, rcp.MES } into grupo
                            select new
                            {
                                grupo.Key.AGE_CODIGO,
                                grupo.Key.PERIODO,
                                grupo.Key.MES,
                                KILOS_MEDIA = Math.Round(grupo.Sum(x => x.TOTAL_LIBRAS ?? 0), 2)
                            }).ToListAsync();

            var presupuestos = await (from p in _context.PRE_VENTAS_ANUAL
                                      where p.PVA_CALIDAD_TX == "MEDIA"
                                      select new
                                      {
                                          p.PVA_COD_AGENTE_FK,
                                          p.PVA_PERIODO_NR,
                                          p.PVA_MES_NR,
                                          PVA_VALOR_NR = p.PVA_VALOR_NR ?? 0
                                      }).ToListAsync();

            var resultado = datosBase.Select(dato =>
            {
                var presupuestoMedia = presupuestos
                    .FirstOrDefault(p => p.PVA_COD_AGENTE_FK == dato.AGE_CODIGO
                                         && p.PVA_PERIODO_NR == dato.PERIODO
                                         && p.PVA_MES_NR == dato.MES)?.PVA_VALOR_NR ?? 0;

                if (presupuesto.DIA != null && presupuesto.DIA != 0 && presupuesto.DIA != -1)
                {
                    int diasmes = DateTime.DaysInMonth(presupuesto.PERIODO ?? 0, presupuesto.MES ?? 0);
                    int dias = (presupuesto.DIAF ?? 0) - (presupuesto.DIA - 1 ?? 0);
                    if (dias > 0)
                    {
                        presupuestoMedia = (presupuestoMedia / diasmes) * dias;
                    }
                    else
                    {
                        presupuestoMedia = (presupuestoMedia / diasmes);
                    }

                }



                var cumplimientoMedia = presupuestoMedia == 0
                    ? 0
                    : Math.Round((dato.KILOS_MEDIA / presupuestoMedia) * 100, 2);

                return new
                {
                   
                    dato.KILOS_MEDIA,
                    PRESUPUESTO_MEDIA = Math.Round(presupuestoMedia, 2),
                    CUMPLIMIENTO_MEDIA = cumplimientoMedia
                };
            }).ToList();

            // Paso 4: Preparar la respuesta
            response.Data = resultado;
            response.Success = true;
            response.Message = "Datos obtenidos exitosamente.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener los datos: {ex.Message}";
            response.Data = null;
        }

        return response;
    }

    public async Task<ServiceResponse<object>> GetPreXCanalPeAsync(AuxPresupuesto presupuesto)
    {
        var response = new ServiceResponse<object>();
        try
        {

            var datos = await (from rcp in _context.REP_CANTIDADES_VENTAS_2009
                               join c in _context.CLIENTE on rcp.CLI_CODIGO equals c.CLI_CODIGO
                               join t in _context.TESTABLECI on c.CLI_ESTABLECIMIENTO equals t.TES_CODIGO
                               where rcp.PERIODO == presupuesto.PERIODO
                                     && rcp.MES == presupuesto.MES
                                     && rcp.AGE_CODIGO == presupuesto.AGENTE
                                     && rcp.CLASIFICACION != "NA"
                               group rcp by t.TES_NOMBRE into grupo
                               select new
                               {
                                   TES_NOMBRE = grupo.Key,
                                   KILOS = grupo.Sum(x => x.TOTAL_LIBRAS ?? 0),
                                   MONTO = grupo.Sum(x => x.TOTAL_CON_DESCUENTOS ?? 0),
                                   CLIENTES = grupo.Select(x => x.CLI_CODIGO).Distinct().Count()
                               })
                            .OrderBy(x => x.TES_NOMBRE)
                            .ToListAsync();

            // Preparar los detalles
            var detalles = datos.Select(d => new DetalleCanal
            {
                Canal = d.TES_NOMBRE,
                Kilos = d.KILOS,
                Monto = d.MONTO,
                Clientes = d.CLIENTES
            }).ToList();

            // Calcular totales
            var totalKilos = detalles.Sum(x => x.Kilos);
            var totalMonto = detalles.Sum(x => x.Monto);
            var totalClientes = detalles.Sum(x => x.Clientes);

            // Preparar la respuesta con cabecera y detalles
            var resultado = new
            {
                Agente = presupuesto.AGENTE,
                Mes = presupuesto.MES,
                Periodo = presupuesto.PERIODO,
                DetallesC = detalles,
                TotalKilos = totalKilos,
                TotalMonto = totalMonto,
                TotalClientes = totalClientes
            };

            if (!detalles.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL MES SELECCIONADO";
                return response;
            }
            response.Data = resultado;
            response.Success = true;
            response.Message = "Datos obtenidos exitosamente.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener los datos: {ex.Message}";
            response.Data = null;
        }
        return response;
    }

}
