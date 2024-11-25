using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Services;

public class DevolucionesService : IDevolucionesService
{

    private readonly ApplicationDbContext _context;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;

    public DevolucionesService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DevolucionesService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }


    public async Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from rv in _context.REP_VENTAS_INT_60
                                join cl in _context.CLIENTE on rv.CCO_CODCLIPRO equals cl.CLI_CODIGO
                                where cl.CLI_AGENTE == cliente.CLI_AGENTE && rv.CCO_CODCLIPRO == cliente.CLI_CODIGO
                                select new
                                {
                                    PRO_NOMBRE = rv.PRO_ID + ". " + rv.PRO_NOMBRE,
                                    rv.PRO_CODIGO,
                                    rv.PRO_ID,
                                    rv.UMD_CODIGO,
                                    rv.UMD_ID,
                                    rv.CCO_CODCLIPRO
                                })
                                .Distinct()
                                .OrderBy(x => x.PRO_ID)
                                .ToListAsync();



            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL CLIENTE SELECCIONADO";
            }

            response.Data = result;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetProDevMotivoseAsync()
    {
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from m in _context.REP_MOTIVOS_DEV
                                where m.TDE_EMPRESA == p_empresa && (m.TDE_INACTIVO ?? 0) == 0
                                select new
                                {
                                    TDE_NOMBRE = m.TDE_ID + ".- " + m.TDE_NOMBRE,
                                    TDE_CODIGO = m.TDE_CODIGO.ToString()
                                })
                                .ToListAsync();




            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL CLIENTE SELECCIONADO";
            }

            response.Data = result;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
        }
        return response;
    }


}
