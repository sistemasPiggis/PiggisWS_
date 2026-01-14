using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.SecurityNamespace;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Firebase;
using PIGGISWS.Services.Utils;
using PIGGISWS.Views.Marcaciones;
using System.Globalization;

namespace PIGGISWS.Services;

public class MarcacionService: IMarcacionService
{

    private readonly ApplicationDbContext _context;
    private readonly ILogger<MarcacionService> _logger;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;

    public MarcacionService(ApplicationDbContext context, ILogger<MarcacionService> logger)
    {
        _context = context;
        _logger = logger;
        GetParametros();
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DevolucionesService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }

    public async Task<ServiceResponse<object>> GetHoraAsync()
    {
        DateTime time;
        try
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT SYSDATE FROM dual";
                await _context.Database.OpenConnectionAsync();

                var result = await command.ExecuteScalarAsync();
                time = Convert.ToDateTime(result);
            }
            respuesta.Data = time;
            respuesta.Success = true;
            respuesta.Message = "Hora recuperada del servidor";
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Data = null;
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            _logger.LogError("ERROR AL RECUPERAR LA HORA: " + ex.ToString());
            return respuesta;
        }
    }

    public async Task<ServiceResponse<object>> CreateMarcacionAsync(Tmp_Marcacion_Agente marcacion_Agente)
    {

        decimal mar_codigo = 0;
       
        string coordenadaUsada = "";
        string tipoMovimiento = "";
        try
        {

            string FormatearCoordenadas(string coordenadas)
            {
                if (string.IsNullOrEmpty(coordenadas))
                {
                    return coordenadas;
                }


                var partes = coordenadas.Split(new[] { ", " }, StringSplitOptions.None);

              
                if (partes.Length == 2)
                {
                   
                    return $"{partes[0].Replace(',', '.')}, {partes[1].Replace(',', '.')}";
                }


                return coordenadas;
            }


            marcacion_Agente.UBICACION1 = FormatearCoordenadas(marcacion_Agente.UBICACION1);
            marcacion_Agente.UBICACION2 = FormatearCoordenadas(marcacion_Agente.UBICACION2);
            marcacion_Agente.UBICACION3 = FormatearCoordenadas(marcacion_Agente.UBICACION3);
            marcacion_Agente.UBICACION4 = FormatearCoordenadas(marcacion_Agente.UBICACION4);


            var registrosExistente = await _context.TMP_MARCACION_AGENTE
             .Where(x => x.AGE_CODIGO == marcacion_Agente.AGE_CODIGO
                                       && x.MAR_FECHA.Date == marcacion_Agente.MAR_FECHA.Date
                                       && x.ID_EMPRESA == marcacion_Agente.ID_EMPRESA).ToListAsync();
            var registroExistente = registrosExistente.FirstOrDefault();

            if (registroExistente == null)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT DATA_USR.TMP_MARCACION_AGENTE_SEQ.NEXTVAL FROM dual";
                    await _context.Database.OpenConnectionAsync();

                    var result = await command.ExecuteScalarAsync();
                    mar_codigo = Convert.ToDecimal(result);
                }

                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA1_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION1))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    marcacion_Agente.ENTRADA1 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.ENTRADA1_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA1_MOVIL).ToString("HH:mm");
                    coordenadaUsada = marcacion_Agente.UBICACION1;
                    tipoMovimiento = "ENTRADA 1";
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA1_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION2))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    marcacion_Agente.SALIDA1 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.SALIDA1_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA1_MOVIL).ToString("HH:mm");
                    coordenadaUsada = marcacion_Agente.UBICACION2;
                    tipoMovimiento = "SALIDA 1";
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA2_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION3))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    marcacion_Agente.ENTRADA2 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.ENTRADA2_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA2_MOVIL).ToString("HH:mm");
                    coordenadaUsada = marcacion_Agente.UBICACION3;
                    tipoMovimiento = "ENTRADA 2";
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA2_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION4))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }

                    marcacion_Agente.SALIDA2 = DateTime.Now.ToString("HH:mm:ss");
                    marcacion_Agente.SALIDA2_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA2_MOVIL).ToString("HH:mm");
                    coordenadaUsada = marcacion_Agente.UBICACION4;
                    tipoMovimiento = "SALIDA 2";
                }

                marcacion_Agente.ID_MARCACION = mar_codigo;
                marcacion_Agente.MAR_FECHA = marcacion_Agente.MAR_FECHA.Date;
                _context.TMP_MARCACION_AGENTE.Add(marcacion_Agente);
            }
            else
            {

                if (string.IsNullOrEmpty(registroExistente.ENTRADA1_MOVIL) && !string.IsNullOrEmpty(marcacion_Agente.ENTRADA1_MOVIL))

                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION1))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    registroExistente.ENTRADA1_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA1_MOVIL).ToString("HH:mm");
                    registroExistente.ENTRADA1 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION1 = marcacion_Agente.UBICACION1;
                    coordenadaUsada = marcacion_Agente.UBICACION1;
                    tipoMovimiento = "ENTRADA 1";
                }
                if (string.IsNullOrEmpty(registroExistente.SALIDA1_MOVIL) && !string.IsNullOrEmpty(marcacion_Agente.SALIDA1_MOVIL))

                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION2))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    registroExistente.SALIDA1_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA1_MOVIL).ToString("HH:mm");
                    registroExistente.SALIDA1 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION2 = marcacion_Agente.UBICACION2;
                    coordenadaUsada = marcacion_Agente.UBICACION2;
                    tipoMovimiento = "SALIDA 1";
                }
                if (string.IsNullOrEmpty(registroExistente.ENTRADA2_MOVIL) && !string.IsNullOrEmpty(marcacion_Agente.ENTRADA2_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION3))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    registroExistente.ENTRADA2_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA2_MOVIL).ToString("HH:mm");
                    registroExistente.ENTRADA2 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION3 = marcacion_Agente.UBICACION3;
                    coordenadaUsada = marcacion_Agente.UBICACION3;
                    tipoMovimiento = "ENTRADA 2";
                }
                if (string.IsNullOrEmpty(registroExistente.SALIDA2_MOVIL) && !string.IsNullOrEmpty(marcacion_Agente.SALIDA2_MOVIL))
                {
                    if (string.IsNullOrEmpty(marcacion_Agente.UBICACION4))
                    {
                        return new ServiceResponse<object> { Success = false, Message = "REVISE LOS PERMISOS DE LA APP, NO SE HAN INGRESADO COORDENADAS" };
                    }
                    registroExistente.SALIDA2_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA2_MOVIL).ToString("HH:mm");
                    registroExistente.SALIDA2 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION4 = marcacion_Agente.UBICACION4;

                    coordenadaUsada = marcacion_Agente.UBICACION4;
                    tipoMovimiento = "SALIDA 2";
                }

            }

            await _context.SaveChangesAsync();

            string estadoCerca = "";
            if (!string.IsNullOrEmpty(coordenadaUsada))
            {
                
                estadoCerca = await VerificarEstadoGeocerca(
                    marcacion_Agente.ID_EMPRESA,
                    marcacion_Agente.AGE_CODIGO,
                    coordenadaUsada
                );

               
               
            }
            var ultimoRegistro = (registroExistente == null) ? marcacion_Agente : registroExistente;
            respuesta.Data = ultimoRegistro;
            respuesta.Success = true;
            if (!string.IsNullOrEmpty(estadoCerca))
            {
                respuesta.Message = $"Marcación guardada. Ubicación: {estadoCerca}";
            }
            else
            {
                respuesta.Message = "Marcación guardada correctamente.";
            }
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Data = null;
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            _logger.LogError("ERROR AL GUARDAR MARCACIÓN: " + ex.ToString());
            return respuesta;
        }
    }


    public async Task<ServiceResponse<object>> GetMarcacionesxAgenteAsync(decimal agente)
    {
        var response = new ServiceResponse<object>();
        DateTime _fecha = DateTime.Now;
        try
        {
            var marcaciones = await _context.TMP_MARCACION_AGENTE
                .Where(m => m.AGE_CODIGO == agente && m.ID_EMPRESA == p_empresa && m.MAR_FECHA.Date == _fecha.Date)
                .ToListAsync();
            var marcacion = marcaciones.FirstOrDefault();
            if (marcacion != null)
                marcacion.MAR_FECHA = marcacion.MAR_FECHA.Date;
            

            if (marcacion == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "No se encontraron marcaciones para el agente.";
                return response;
            }
            response.Data = marcacion;
            response.Success = true;
            response.Message = "Marcaciones encontradas.";
        }
        catch (Exception ex)
        {
            response.Data = null;
            response.Success = false;
            response.Message = "Error al obtener las marcaciones: " + ex.ToString();
            _logger.LogError("GetMarcacionesxAgenteAsync: " + ex.ToString());
        }
        return response;
    }



    //// VALIDAR UBICACIÒN DENTRO DE GEO CERCAS
    private async Task<string> VerificarEstadoGeocerca(int idEmpresa, decimal ageCodigo, string coordenadasString)
    {
        if (string.IsNullOrEmpty(coordenadasString)) return "SIN COORDENADAS";

        try
        {
            
            var partes = coordenadasString.Split(',');
            if (partes.Length != 2) return "ERROR COORDENADAS";

            double latUsuario = double.Parse(partes[0].Trim().Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            double lonUsuario = double.Parse(partes[1].Trim().Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

            System.DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

            // Crea un objeto CultureInfo en español
            CultureInfo ci = new CultureInfo("es-ES");
            string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);
          
            string dayformateado = dayName.ToUpper();
            dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);


            var todosLosPuntos = await _context.MAP_CERCA_AGENTE 
                .Where(x => x.ID_EMPRESA == idEmpresa
                         && x.AGE_CODIGO == ageCodigo
                         && x.DIA == dayformateado)
                .Select(x => new { x.LATITUD, x.LONGITUD, x.ID_MAP_CERCA, x.SECUENCIA }) 
                .ToListAsync();

            if (todosLosPuntos == null || !todosLosPuntos.Any()) return "SIN RUTA CONFIGURADA";

            
            var poligonos = todosLosPuntos.GroupBy(x => x.SECUENCIA);

            bool estaDentroDeAlguna = false;

            foreach (var grupo in poligonos)
            {
               
                var listaPuntos = grupo.OrderBy(x => x.ID_MAP_CERCA).Cast<dynamic>().ToList();

              
                if (PuntoEstaEnPoligono(latUsuario, lonUsuario, listaPuntos))
                {
                    estaDentroDeAlguna = true;
                    break; 
                }
            }

            return estaDentroDeAlguna ? "DENTRO DE RUTA" : "FUERA DE RUTA";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calculando geocerca: {ex.Message}");
            return "ERROR VALIDACION";
        }
    }




    private bool PuntoEstaEnPoligono(double latitud, double longitud, List<dynamic> poligono)
    {
        bool inside = false;

       
        double ToDouble(object valor)
        {
            if (valor == null) return 0;
            string valStr = valor.ToString().Trim().Replace(',', '.'); // Asegura formato 0.00
            if (double.TryParse(valStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return 0;
        }

        for (int i = 0, j = poligono.Count - 1; i < poligono.Count; j = i++)
        {
            // CORRECCIÓN AQUÍ: Usamos el helper ToDouble en lugar de (double)
            double xi = ToDouble(poligono[i].LATITUD);
            double yi = ToDouble(poligono[i].LONGITUD);
            double xj = ToDouble(poligono[j].LATITUD);
            double yj = ToDouble(poligono[j].LONGITUD);

            bool intersect = ((yi > longitud) != (yj > longitud))
                && (latitud < (xj - xi) * (longitud - yi) / (yj - yi) + xi);
            if (intersect) inside = !inside;
        }
        return inside;
    }
    ///




}
