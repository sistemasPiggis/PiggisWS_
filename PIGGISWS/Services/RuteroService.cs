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

    public async Task<ServiceResponse<object>> SetRuteroPedidoAsync(decimal cliente, decimal agente, DateTime fecha, decimal zona)
    {
        try
        {
            var response = new ServiceResponse<object>();
            var startOfDay = fecha.Date; // Inicio del día actual
            var endOfDay = fecha.Date.AddDays(1);
            var ruteros = await _context.RUTERO
                             .Where(r => r.RUT_CLIENTE == cliente
                                          && r.RUT_AGENTE == agente
                                          && r.RUT_FECHA >= startOfDay
                                          && r.RUT_FECHA < endOfDay)
                             .ToListAsync();
            var rutero = ruteros.FirstOrDefault();
            if (rutero is null)
            {
                var _rutero = new Rutero
                {
                    RUT_EMPRESA = p_empresa,
                    RUT_FECHA = fecha.Date,
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
               int ruteroUpdate = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                            UPDATE RUTERO
                            SET RUT_PEDIDO = 1, RUT_VISITA = 1
                            WHERE RUT_CLIENTE = {cliente}
                              AND RUT_AGENTE = {agente}
                              AND RUT_FECHA >= {startOfDay}
                              AND RUT_FECHA < {endOfDay}
");

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
        catch (Exception ex)
        {
            return new ServiceResponse<object>
            {
                Success = false,
                Message = "ERROR AL GUARDAR RUTERO: " + ex.Message,
                Data = null
            };
        }
    }


    public async Task<ServiceResponse<object>> ValidaHoraPedidoAsync(decimal agente, DateTime fecha)
    {
        var response = new ServiceResponse<object>();
        System.DayOfWeek dayOfWeek = fecha.DayOfWeek;
        TimeSpan _horapedido = DateTime.Now.TimeOfDay;
        
        TimeSpan horapedido = new TimeSpan(_horapedido.Hours, _horapedido.Minutes, 0);
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
                response.Message = "PEDIDO INGRESARO FUERA DE HORARIO SU HORARIO ES:  "+ horaInicio +" - " + horaFin+ " + HORA DEL SERVIDO "+dayformateado+"   ";
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

    public async Task<ServiceResponse<object>> SetVisitaAsync(Rutero rutero)
    {
        var response = new ServiceResponse<object>();
        //DateTime hoy = DateTime.Now;
        try
        {
            decimal agente = rutero.RUT_AGENTE;
            decimal cliente = rutero.RUT_CLIENTE;
            decimal zona = rutero.RUT_ZONA;
            DateTime hoy  = rutero.RUT_FECHA;
            var startOfDay = hoy.Date; // Inicio del día actual
            var endOfDay = hoy.Date.AddDays(1);
            if (agente != 0 && cliente > 0)
            {
                var clientes = await _context.CLIENTE.Where(r => r.CLI_CODIGO == cliente).ToListAsync();
                var _cliente = clientes.FirstOrDefault();
                var ruteros = await _context.RUTERO
                         .Where(r => r.RUT_CLIENTE == cliente
                                      && r.RUT_AGENTE == agente
                                      && r.RUT_FECHA >= startOfDay
                                      && r.RUT_FECHA < endOfDay)
                         .ToListAsync();
                var _rutero = ruteros.FirstOrDefault();
                if (_cliente != null)
                {
                    if (_rutero == null)
                    {
                        var rutero_ = new Rutero
                        {
                            RUT_AGENTE = agente,
                            RUT_CLIENTE = cliente,
                            RUT_EMPRESA = 1,
                            RUT_FECHA = hoy.Date,
                            RUT_VISITA = 1,
                            RUT_ZONA = zona
                        };

                       await _context.RUTERO.AddAsync(rutero_);
                       int ruterosave = await _context.SaveChangesAsync();
                        if (ruterosave > 0)
                        {
                            
                            response.Success = true;
                            response.Message = "VISITA REGISTRADA EXISTOSAMENTE";
                            response.Data = rutero_;
                            return response;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message = "ERROR AL GRABAR VISITA";
                            response.Data = rutero_;
                            return response;
                        }
                    }
                    else
                    {
                       response.Success = true;
                       response.Message = "YA SE REGISTRO PEDIDO, COBRO O VISITA";
                       response.Data = null;
                        return response;
                    }
                }
            }
            else
            {
                response.Success = true;
                response.Message = "NO SE TIENEN DATOS DE CLIENTE VERIFICAR DATOS";
                response.Data = null;
                return response;
            }
        }
        catch (Exception ex)
        { 
            response.Success = true;
            response.Message = ex.Message;
            response.Data = ex;
        }
            return response;
    }
}
