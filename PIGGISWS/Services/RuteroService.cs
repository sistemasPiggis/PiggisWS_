using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Services.Utils;
using System.Globalization;

namespace PIGGISWS.Services;

public class RuteroService : IRuteroService
{
    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;

    public RuteroService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "RuteroService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task<ServiceResponse<object>> SetRuteroPedidoAsync(decimal cliente, int agente, DateTime fecha, decimal zona)
    {
        var response = new ServiceResponse<object>();
        var ruteros = await _context.RUTERO.Where(r => r.RUT_CLIENTE == cliente && r.RUT_AGENTE == agente && r.RUT_FECHA == fecha).ToListAsync();
        var rutero = ruteros.FirstOrDefault();
        if ( rutero is null)
        {
            var _rutero = new Rutero
            {
                RUT_EMPRESA = p_empresa,
                RUT_FECHA = fecha,
                RUT_AGENTE = agente,
                RUT_CLIENTE = cliente,
                RUT_ZONA = zona,
                RUT_PEDIDO = 1,
                RUT_VISITA = 1
                
            };
            await _context.RUTERO.AddAsync(_rutero);
            int ruterosave = await _context.SaveChangesAsync();
            if (ruterosave > 0)
            {
                response.Success = true;
                response.Data = _rutero;
                response.Message = "RUTERO GUARDADO EXITOSAMENTE";
            }
            else
            {
                response.Success = false;
                response.Data = null;
                response.Message = "ERROR AL INSERTAR EL RUTERO";
            }
        }
        else
        {
            rutero.RUT_PEDIDO = 1; // Ejemplo: si quieres incrementar el valor
            rutero.RUT_VISITA = 1; // Ejemplo: si quieres incrementar el valor
            int ruteroUpdate = await _context.SaveChangesAsync();

            if (ruteroUpdate > 0)
            {
                response.Success = true;
                response.Data = rutero;
                response.Message = "RUTERO ACTUALIZADO EXITOSAMENTE";
            }
            else
            {
                response.Success = false;
                response.Data = null;
                response.Message = "ERROR AL ACTUALIZAR EL RUTERO";
            }
        }

        return response;
    }


    public async Task<ServiceResponse<object>> ValidaHoraPedidoAsync(int agente, DateTime fecha)
    {
        var response = new ServiceResponse<object>();
        System.DayOfWeek dayOfWeek = fecha.DayOfWeek;
        TimeSpan horapedido = fecha.TimeOfDay;
        CultureInfo ci = new CultureInfo("es-ES");

        // Obtiene el nombre del día de la semana en español
        string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);

        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);

        var _hora1 = await _context.AGENTE_CALENDARIO_PEDIDO.Where(c=>c.AGE_ID_EMPLEADO_FK == agente && c.AGE_DIA == dayformateado)
            .Select(c=> c.AGE_HORA_INICIO)
            .ToListAsync();
        var hora1 = _hora1.FirstOrDefault();
        var _hora2 = await _context.AGENTE_CALENDARIO_PEDIDO.Where(c => c.AGE_ID_EMPLEADO_FK == agente && c.AGE_DIA == dayformateado)
            .Select(c => c.AGE_HORA_CIERRE)
            .ToListAsync();
        var hora2 = _hora2.FirstOrDefault();
        if (hora1 != null && hora2 != null) // si es nulo no tiene restricción de horario sabado y domingo 
        {
            // Convertir las cadenas de hora a TimeSpan
            TimeSpan horaInicio = TimeSpan.Parse(hora1);
            TimeSpan horaFin = TimeSpan.Parse(hora2);

            if (horapedido >= horaInicio && horapedido <= horaFin)
            {
                response.Success = true;
                response.Data = true;
                response.Message = "PEDIDO INGRESARO EN HORARIO";
            }
            else
            {
                response.Success = false;
                response.Data = false;
                response.Message = "PEDIDO INGRESARO FUERA DE HORARIO SU HORARIO ES:  "+ horaInicio +" - " + horaFin+" ";
            }

        }
        else
        {
            response.Success = true;
            response.Message = "Agente Sin restricción";
            response.Data = true;
           
        }

        return response;

    }


}
