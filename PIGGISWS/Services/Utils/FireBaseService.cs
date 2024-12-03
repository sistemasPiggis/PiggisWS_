using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.SecurityNamespace;
using Microsoft.OpenApi.Writers;
using PIGGISWS.Data;
using PIGGISWS.Models;
using PIGGISWS.Models.Firebase;

namespace PIGGISWS.Services.Utils;


/// <summary>
/// Clase para interactuar con la base de datos de Firebase, en este caso se utiliza para obtener las marcaciones de los agentes 
/// que no han podido marcar normalmente, migra cada cierto tiempo
/// </summary>
public class FireBaseService
{
    private readonly FirebaseClient _firebaseClient;

    private readonly ApplicationDbContext _context;
    private readonly ILogger<FireBaseService> _logger;

    public FireBaseService(IConfiguration configuration, ApplicationDbContext context, ILogger<FireBaseService> logger)
    {
        _context = context;
    _logger = logger;
        var firebaseUrl = configuration["Firebase:DatabaseUrl"];
        var serviceAccountKeyPath = configuration["Firebase:ServiceAccountKeyPath"];

        GoogleCredential credential;
        using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped("https://www.googleapis.com/auth/firebase.database");
        }

        var token = credential.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;

        _firebaseClient = new FirebaseClient(
            firebaseUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(token)
            });
    }

    public async Task<List<Marcacion>> GetMarcacionesAsync()
    {
        try
        {


            var hoy = DateTime.Today.AddDays(-4);
            var oneWeekAgo = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
            var marcaciones = (await _firebaseClient
                        .Child("marcaciones")
                        .OrderBy("Fecha")
                        
                        .LimitToLast(500000)
                        .OnceAsync<Marcacion>())
                        .Select(item => new Marcacion
                        {
                            Id = item.Key,
                            Fecha = Convert.ToDateTime( item.Key),
                            age_codigo = item.Object.age_codigo,
                            Ubicacion = $"{item.Object.Latitud},{item.Object.Longitud}",
                            Entrada1 = item.Object.Entrada1,
                            Salida1 = item.Object.Salida1,
                            Entrada2 = item.Object.Entrada2,
                            Salida2 = item.Object.Salida2
                        })
                        .OrderByDescending(m => m.Fecha)
                        .ToList();

                if (marcaciones.Any())
                {
                var agentesnomarca = await (from a in _context.AGENTE  /// trae todos los agente activos pues el ERP tambien los muestra
                                              join t in _context.TMP_MARCACION_AGENTE
                                              on new { AGE_CODIGO = (decimal)a.AGE_CODIGO, Fecha = hoy.Date } 
                                              equals new { t.AGE_CODIGO, Fecha = t.MAR_FECHA.Date } into atGroup
                                              from t in atGroup.DefaultIfEmpty()
                                              where a.AGE_INACTIVO == 0 && a.AGE_EMPLEADO != 0
                                              group a by a.AGE_CODIGO into g
                                              select g.Key).ToListAsync();


   
                    if (agentesnomarca.Any())
                    {
                        var marcacionesE1 = marcaciones /// trae las marcaciones 1 de todos los agentes registrados activos
                            .Where(m => m.Entrada1 != null && m.Fecha.Date == hoy.Date
                            && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                            .GroupBy(m => m.age_codigo)
                            .Select(g => g.First())
                            .ToList();


                    var marcacionesS1 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
                         .Where(m => m.Salida1 != null && m.Fecha.Date == hoy.Date
                         && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                         .GroupBy(m => m.age_codigo)
                         .Select(g => g.First())
                         .ToList();

                    var marcacionesE2 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
                       .Where(m => m.Entrada2 != null && m.Fecha.Date == hoy.Date
                       && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                       .GroupBy(m => m.age_codigo)
                       .Select(g => g.First())
                       .ToList();


                    var marcacionesS2 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
                       .Where(m => m.Salida2 != null && m.Fecha.Date == hoy.Date
                       && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                       .GroupBy(m => m.age_codigo)
                       .Select(g => g.First())
                       .ToList();

                    if (marcacionesE1.Any())
                        {

                            foreach (var marcacion in marcacionesE1)
                            {


                            var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
                                                .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo) 
                                                && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
                            var marcacionAgente = _marcacionAgente.FirstOrDefault();

                            if (marcacionAgente != null)
                            {
                                if (marcacionAgente.ENTRADA1_MOVIL == null)
                                {
                                    // El agente existe y ENTRADA1_MOVIL es null - se actualiza
                                    marcacionAgente.ENTRADA1_MOVIL = marcacion.Entrada1;
                                    marcacionAgente.ENTRADA1 = marcacion.Entrada1;
                                    marcacionAgente.UBICACION1 = marcacion.Ubicacion;
                                    _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
                                    _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL: {marcacionAgente.ENTRADA1_MOVIL}, UBICACION1: {marcacionAgente.UBICACION1}, MAR_FECHA: {marcacionAgente.MAR_FECHA}");
                                }
                                else
                                {
                                    // ENTRADA1_MOVIL no es null - no se hace nada
                                    _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL ya registrada.");
                                }
                            }
                            else
                            {
                                //  no existe en la tabla temp - se inserta
                                var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
                                {
                                    ID_EMPRESA = 1,
                                    AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
                                    MAR_FECHA = hoy.Date,
                                    ENTRADA1_MOVIL = marcacion.Entrada1,
                                    UBICACION1 = marcacion.Ubicacion
                                    // Asignar otros campos necesarios si es requerido
                                };
                                _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
                                _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL: {nuevoMarcacionAgente.ENTRADA1_MOVIL}, UBICACION1: {nuevoMarcacionAgente.UBICACION1}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");
                            }

                        }
                            await _context.SaveChangesAsync();
                        
                        }

                    if (marcacionesS1.Any())
                    {

                        foreach (var marcacion in marcacionesS1)
                        {


                            var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
                                                .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
                                                && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
                            var marcacionAgente = _marcacionAgente.FirstOrDefault();

                            if (marcacionAgente != null)
                            {
                                if (marcacionAgente.SALIDA1_MOVIL == null)
                                {
                                    // El agente existe y SALIDA1 es null - se actualiza
                                    marcacionAgente.SALIDA1_MOVIL = marcacion.Salida1;
                                    marcacionAgente.SALIDA1 = marcacion.Salida1;
                                    marcacionAgente.UBICACION2 = marcacion.Ubicacion;
                                    _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
                                    _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA1: {marcacionAgente.SALIDA1_MOVIL}, " +
                                        $"UBICACION1: {marcacionAgente.UBICACION2}, " +
                                        $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
                                }
                                else
                                {
                                    // SALIDA no es null - no se hace nada
                                    _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA 1 ya registrada.");
                                }
                            }
                            else
                            {
                                //  no existe en la tabla temp - se inserta
                                var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
                                {
                                    ID_EMPRESA = 1,
                                    AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
                                    MAR_FECHA = hoy.Date,
                                    SALIDA1_MOVIL = marcacion.Salida1,
                                    UBICACION2 = marcacion.Ubicacion
                                    // Asignar otros campos necesarios si es requerido
                                };
                                _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
                                _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL: {nuevoMarcacionAgente.ENTRADA1_MOVIL}, UBICACION1: {nuevoMarcacionAgente.UBICACION1}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");
                            }

                        }
                        await _context.SaveChangesAsync();

                    }

                    if (marcacionesE2.Any())
                    {

                        foreach (var marcacion in marcacionesE2)
                        {


                            var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
                                                .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
                                                && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
                            var marcacionAgente = _marcacionAgente.FirstOrDefault();

                            if (marcacionAgente != null)
                            {
                                if (marcacionAgente.ENTRADA2_MOVIL == null)
                                {
                                    // El agente existe y SALIDA1 es null - se actualiza
                                    marcacionAgente.ENTRADA2_MOVIL = marcacion.Entrada2;
                                    marcacionAgente.ENTRADA2 = marcacion.Entrada2;
                                    marcacionAgente.UBICACION3 = marcacion.Ubicacion;
                                    _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
                                    _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA1: {marcacionAgente.ENTRADA2_MOVIL}, " +
                                        $"UBICACION1: {marcacionAgente.UBICACION3}, " +
                                        $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
                                }
                                else
                                {
                                    // SALIDA no es null - no se hace nada
                                    _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA 1 ya registrada.");
                                }
                            }
                            else
                            {
                                //  no existe en la tabla temp - se inserta
                                var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
                                {
                                    ID_EMPRESA = 1,
                                    AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
                                    MAR_FECHA = hoy.Date,
                                    ENTRADA2_MOVIL = marcacion.Entrada2,
                                    UBICACION3 = marcacion.Ubicacion
                                    // Asignar otros campos necesarios si es requerido
                                };
                                _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
                                _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}, ENTRADA2_MOVIL: {nuevoMarcacionAgente.ENTRADA2_MOVIL}, " +
                                    $"UBICACION1: {nuevoMarcacionAgente.UBICACION3}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");
                            }

                        }
                        await _context.SaveChangesAsync();

                    }

                    if (marcacionesS2.Any())
                    {

                        foreach (var marcacion in marcacionesS2)
                        {


                            var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
                                                .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
                                                && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
                            var marcacionAgente = _marcacionAgente.FirstOrDefault();

                            if (marcacionAgente != null)
                            {
                                if (marcacionAgente.SALIDA2_MOVIL == null)
                                {
                                    // El agente existe y SALIDA1 es null - se actualiza
                                    marcacionAgente.SALIDA2_MOVIL = marcacion.Salida2;
                                    marcacionAgente.SALIDA2 = marcacion.Salida2;
                                    marcacionAgente.UBICACION3 = marcacion.Ubicacion;
                                    _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
                                    _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA1: {marcacionAgente.ENTRADA2_MOVIL}, " +
                                        $"UBICACION1: {marcacionAgente.UBICACION3}, " +
                                        $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
                                }
                                else
                                {
                                    // SALIDA no es null - no se hace nada
                                    _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                        $"SALIDA 1 ya registrada.");
                                }
                            }
                            else
                            {
                                //  no existe en la tabla temp - se inserta
                                var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
                                {
                                    ID_EMPRESA = 1,
                                    AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
                                    MAR_FECHA = hoy.Date,
                                    ENTRADA2_MOVIL = marcacion.Entrada2,
                                    UBICACION3 = marcacion.Ubicacion
                                    // Asignar otros campos necesarios si es requerido
                                };
                                _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
                                _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}, ENTRADA2_MOVIL: {nuevoMarcacionAgente.ENTRADA2_MOVIL}, " +
                                    $"UBICACION1: {nuevoMarcacionAgente.UBICACION3}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");
                            }

                        }
                        await _context.SaveChangesAsync();

                    }

                }
                
               
            }
            return marcaciones;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las marcaciones de Firebase", ex);

        }
    }



}
