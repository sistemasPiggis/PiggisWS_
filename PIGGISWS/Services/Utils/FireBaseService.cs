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
using System.Reflection;

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
            var hoy = DateTime.Today.AddDays(0);
            var oneWeekAgo = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
            var marcaciones = (await _firebaseClient
                        .Child("marcaciones")
                        .OrderBy("Fecha")
                        .LimitToLast(90000)
                        .OnceAsync<Marcacion>())
                        .Select(item => new Marcacion
                        {
                            Id = item.Key,
                            Fecha = Convert.ToDateTime(item.Key),
                            age_codigo = item.Object.age_codigo,
                            Ubicacion = $"{item.Object.Latitud},{item.Object.Longitud}",
                            Entrada1 = item.Object.Entrada1,
                            Salida1 = item.Object.Salida1,
                            Entrada2 = item.Object.Entrada2,
                            Salida2 = item.Object.Salida2,
                            age_id = item.Object.age_id
                        })
                        .OrderByDescending(m => m.Fecha)
                        .ToList();

            if (marcaciones.Any())
            {
                var agentesnomarca = await (from a in _context.AGENTE
                                            join t in _context.TMP_MARCACION_AGENTE
                                            on new { AGE_CODIGO = (decimal)a.AGE_CODIGO, Fecha = hoy.Date }
                                            equals new { t.AGE_CODIGO, Fecha = t.MAR_FECHA.Date } into atGroup
                                            from t in atGroup.DefaultIfEmpty()
                                            where a.AGE_INACTIVO == 0 && a.AGE_EMPLEADO != 0
                                            group a by a.AGE_CODIGO into g
                                            select g.Key).ToListAsync();

                if (agentesnomarca.Any())
                {
                    var marcacionesE1 = marcaciones
                        .Where(m => m.Entrada1 != null && m.Fecha.Date == hoy.Date
                        && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                        .GroupBy(m => m.age_codigo)
                        .Select(g => g.First())
                        .Distinct()
                        .ToList();

                    var marcacionesS1 = marcaciones
                        .Where(m => m.Salida1 != null && m.Fecha.Date == hoy.Date
                        && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                        .GroupBy(m => m.age_codigo)
                        .Select(g => g.First())
                        .ToList();

                    var marcacionesE2 = marcaciones
                        .Where(m => m.Entrada2 != null && m.Fecha.Date == hoy.Date
                        && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                        .GroupBy(m => m.age_codigo)
                        .Select(g => g.First())
                        .ToList();

                    var marcacionesS2 = marcaciones
                        .Where(m => m.Salida2 != null && m.Fecha.Date == hoy.Date
                        && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
                        .GroupBy(m => m.age_codigo)
                        .Select(g => g.First())
                        .ToList();

                    await ProcesarMarcaciones(marcacionesE1, hoy, "ENTRADA1_MOVIL", "Entrada1", "UBICACION1");
                    await ProcesarMarcaciones(marcacionesS1, hoy, "SALIDA1_MOVIL", "Salida1", "UBICACION2");
                    await ProcesarMarcaciones(marcacionesE2, hoy, "ENTRADA2_MOVIL", "Entrada2", "UBICACION3");
                    await ProcesarMarcaciones(marcacionesS2, hoy, "SALIDA2_MOVIL", "Salida2", "UBICACION4");
                }
            }
            return marcaciones;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las marcaciones de Firebase");
            throw new Exception("Error al obtener las marcaciones de Firebase", ex);
        }
    }

    private async Task ProcesarMarcaciones(List<Marcacion> marcaciones, DateTime hoy, string campoMovil, string campo, string ubicacion)
    {
        try
        {
            decimal id_marcacion = 0;
            foreach (var marcacion in marcaciones)
            {
                var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
                    .AsNoTracking()
                    .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
                    && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
                var marcacionAgente = _marcacionAgente.FirstOrDefault();

                if (marcacionAgente != null)
                {
                    var propiedadMovil = marcacionAgente.GetType().GetProperty(campoMovil);
                    var propiedadCampo = marcacionAgente.GetType().GetProperty(campo, BindingFlags.Public
                        | BindingFlags.Instance | BindingFlags.IgnoreCase); ///ignora mayusculas y minusculas
                    var propiedadCampoMarcacion = marcacion.GetType().GetProperty(campo);
                    var propiedadUbicacion = marcacionAgente.GetType().GetProperty(ubicacion);

                    if (propiedadMovil != null && propiedadCampo != null && propiedadCampoMarcacion != null && propiedadUbicacion != null)
                    {
                        if (propiedadMovil.GetValue(marcacionAgente) == null)
                        {


                            var valorCampo = propiedadCampoMarcacion.GetValue(marcacion);

                            // Convertir valorCampo al tipo de propiedadMovil si es necesario
                            if (valorCampo != null)
                            {
                                var valorConvertido = Convert.ChangeType(valorCampo, propiedadMovil.PropertyType);
                                propiedadMovil.SetValue(marcacionAgente, valorConvertido);
                                propiedadCampo.SetValue(marcacionAgente, valorConvertido); // Actualizar también el campo sin '_MOVIL'
                            }

                            // Asignar Ubicacion
                            propiedadUbicacion.SetValue(marcacionAgente, marcacion.Ubicacion);

                            // Actualizar fecha de modificación
                            marcacionAgente.MOD_FECHA = DateTime.Now;
                            _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);

                            _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
                                $"{campoMovil}: {propiedadMovil.GetValue(marcacionAgente)}, " +
                                $"{ubicacion}: {propiedadUbicacion.GetValue(marcacionAgente)}, MAR_FECHA: {marcacionAgente.MAR_FECHA}");
                        }
                        else
                        {
                            _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, {campoMovil} ya registrada.");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Propiedad no encontrada para {campoMovil}, {campo} o {ubicacion}");
                    }
                }

                else
                {

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "select TMP_MARCACION_AGENTE_SEQ.NEXTVAL FROM dual";
                        await _context.Database.OpenConnectionAsync();

                        var result = await command.ExecuteScalarAsync();
                        id_marcacion = Convert.ToDecimal(result);
                    }
                    var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
                    {
                        ID_MARCACION = id_marcacion,
                        ID_EMPRESA = 1,
                        AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
                        MAR_FECHA = hoy.Date,
                        ENTRADA1 = campoMovil == "ENTRADA1_MOVIL" ? marcacion.Entrada1 : null,
                        ENTRADA1_MOVIL = campoMovil == "ENTRADA1_MOVIL" ? marcacion.Entrada1 : null,
                        SALIDA1 = campoMovil == "SALIDA1_MOVIL" ? marcacion.Salida1 : null,
                        SALIDA1_MOVIL = campoMovil == "SALIDA1_MOVIL" ? marcacion.Salida1 : null,
                        ENTRADA2 = campoMovil == "ENTRADA2_MOVIL" ? marcacion.Entrada2 : null,
                        ENTRADA2_MOVIL = campoMovil == "ENTRADA2_MOVIL" ? marcacion.Entrada2 : null,
                        SALIDA2 = campoMovil == "SALIDA2_MOVIL" ? marcacion.Salida2 : null,
                        SALIDA2_MOVIL = campoMovil == "SALIDA2_MOVIL" ? marcacion.Salida2 : null,
                        UBICACION1 = ubicacion == "UBICACION1" ? marcacion.Ubicacion : null,
                        UBICACION2 = ubicacion == "UBICACION2" ? marcacion.Ubicacion : null,
                        UBICACION3 = ubicacion == "UBICACION3" ? marcacion.Ubicacion : null,
                        UBICACION4 = ubicacion == "UBICACION4" ? marcacion.Ubicacion : null,
                        MOD_FECHA = DateTime.Now
                    };



                    var trackedEntity = _context.TMP_MARCACION_AGENTE.Local
                   .FirstOrDefault(m => m.AGE_CODIGO == nuevoMarcacionAgente.AGE_CODIGO && m.MAR_FECHA == nuevoMarcacionAgente.MAR_FECHA);

                    if (trackedEntity != null)
                    {
                        _context.Entry(trackedEntity).State = EntityState.Detached;
                    }
                    _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
                    _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}," +
                        $" {campoMovil}: {nuevoMarcacionAgente.GetType().GetProperty(campoMovil).GetValue(nuevoMarcacionAgente)}, " +
                        $"{ubicacion}: {nuevoMarcacionAgente.GetType().GetProperty(ubicacion).GetValue(nuevoMarcacionAgente)}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");

                }
            }
            await _context.SaveChangesAsync();
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar marcaciones");
            throw new Exception("Error al procesar marcaciones", ex);
        }
    }
}



    //public async Task<List<Marcacion>> GetMarcacionesAsync()
    //{
    //    try
    //    {


    //        var hoy = DateTime.Today.AddDays(0);
    //        var oneWeekAgo = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
    //        var marcaciones = (await _firebaseClient
    //                    .Child("marcaciones")
    //                    .OrderBy("Fecha")

    //                    .LimitToLast(500000)
    //                    .OnceAsync<Marcacion>())
    //                    .Select(item => new Marcacion
    //                    {
    //                        Id = item.Key,
    //                        Fecha = Convert.ToDateTime(item.Key),
    //                        age_codigo = item.Object.age_codigo,
    //                        Ubicacion = $"{item.Object.Latitud},{item.Object.Longitud}",
    //                        Entrada1 = item.Object.Entrada1,
    //                        Salida1 = item.Object.Salida1,
    //                        Entrada2 = item.Object.Entrada2,
    //                        Salida2 = item.Object.Salida2,
    //                        age_id = item.Object.age_id
    //                    })
    //                    .OrderByDescending(m => m.Fecha)
    //                    .ToList();


    //        if (marcaciones.Any())
    //        {

    //            var agentesnomarca = await (from a in _context.AGENTE  /// trae todos los agente activos pues el ERP tambien los muestra
    //                                        join t in _context.TMP_MARCACION_AGENTE
    //                                        on new { AGE_CODIGO = (decimal)a.AGE_CODIGO, Fecha = hoy.Date }
    //                                        equals new { t.AGE_CODIGO, Fecha = t.MAR_FECHA.Date } into atGroup
    //                                        from t in atGroup.DefaultIfEmpty()
    //                                        where a.AGE_INACTIVO == 0 && a.AGE_EMPLEADO != 0
    //                                        group a by a.AGE_CODIGO into g
    //                                        select g.Key).ToListAsync();



    //            if (agentesnomarca.Any())
    //            {
    //                var marcacionesE1 = marcaciones /// trae las marcaciones 1 de todos los agentes registrados activos
    //                    .Where(m => m.Entrada1 != null && m.Fecha.Date == hoy.Date
    //                    && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
    //                    .GroupBy(m => m.age_codigo)
    //                    .Select(g => g.First())
    //                    .ToList();


    //                var marcacionesS1 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
    //                     .Where(m => m.Salida1 != null && m.Fecha.Date == hoy.Date
    //                     && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
    //                     .GroupBy(m => m.age_codigo)
    //                     .Select(g => g.First())
    //                     .ToList();

    //                var marcacionesE2 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
    //                   .Where(m => m.Entrada2 != null && m.Fecha.Date == hoy.Date
    //                   && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
    //                   .GroupBy(m => m.age_codigo)
    //                   .Select(g => g.First())
    //                   .ToList();


    //                var marcacionesS2 = marcaciones /// trae las marcaciones 2 de todos los agentes registrados activos
    //                   .Where(m => m.Salida2 != null && m.Fecha.Date == hoy.Date
    //                   && agentesnomarca.Contains(Convert.ToInt32(m.age_codigo)))
    //                   .GroupBy(m => m.age_codigo)
    //                   .Select(g => g.First())
    //                   .ToList();

    //                if (marcacionesE1.Any())
    //                {

    //                    foreach (var marcacion in marcacionesE1)
    //                    {


    //                        var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
    //                            .AsNoTracking()
    //                                            .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
    //                                            && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
    //                        var marcacionAgente = _marcacionAgente.FirstOrDefault();

    //                        if (marcacionAgente != null)
    //                        {
    //                            if (marcacionAgente.ENTRADA1_MOVIL == null)
    //                            {
    //                                // El agente existe y ENTRADA1_MOVIL es null - se actualiza
    //                                marcacionAgente.ENTRADA1_MOVIL = marcacion.Entrada1;
    //                                marcacionAgente.ENTRADA1 = marcacion.Entrada1;
    //                                marcacionAgente.UBICACION1 = marcacion.Ubicacion;
    //                                _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
    //                                _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL: {marcacionAgente.ENTRADA1_MOVIL}, UBICACION1: {marcacionAgente.UBICACION1}, MAR_FECHA: {marcacionAgente.MAR_FECHA}");
    //                            }
    //                            else
    //                            {
    //                                // ENTRADA1_MOVIL no es null - no se hace nada
    //                                _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL ya registrada.");
    //                            }
    //                        }
    //                        else
    //                        {
    //                            //  no existe en la tabla temp - se inserta
    //                            var nuevoMarcacionAgente = new Tmp_Marcacion_Agente
    //                            {
    //                                ID_EMPRESA = 1,
    //                                AGE_CODIGO = Convert.ToDecimal(marcacion.age_codigo),
    //                                MAR_FECHA = hoy.Date,
    //                                ENTRADA1_MOVIL = marcacion.Entrada1,
    //                                UBICACION1 = marcacion.Ubicacion
    //                                // Asignar otros campos necesarios si es requerido
    //                            };

    //                            var trackedEntity = _context.TMP_MARCACION_AGENTE.Local
    //                            .FirstOrDefault(m => m.AGE_CODIGO == nuevoMarcacionAgente.AGE_CODIGO && m.MAR_FECHA == nuevoMarcacionAgente.MAR_FECHA);

    //                            if (trackedEntity == null)
    //                            {
    //                                _context.TMP_MARCACION_AGENTE.Add(nuevoMarcacionAgente);
    //                                _logger.LogInformation($"Insertado AGE_CODIGO: {nuevoMarcacionAgente.AGE_CODIGO}, ENTRADA1_MOVIL: {nuevoMarcacionAgente.ENTRADA1_MOVIL}, UBICACION1: {nuevoMarcacionAgente.UBICACION1}, MAR_FECHA: {nuevoMarcacionAgente.MAR_FECHA}");
    //                            }
    //                            else
    //                            {
    //                                _logger.LogInformation($"Entidad ya rastreada AGE_CODIGO: {trackedEntity.AGE_CODIGO}, MAR_FECHA: {trackedEntity.MAR_FECHA}");
    //                            }

    //                        }

    //                    }
    //                    await _context.SaveChangesAsync();

    //                }

    //                if (marcacionesS1.Any())
    //                {

    //                    foreach (var marcacion in marcacionesS1)
    //                    {


    //                        var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
    //                            .AsNoTracking()
    //                                            .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
    //                                            && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
    //                        var marcacionAgente = _marcacionAgente.FirstOrDefault();

    //                        if (marcacionAgente != null)
    //                        {
    //                            if (marcacionAgente.SALIDA1_MOVIL == null)
    //                            {
    //                                // El agente existe y SALIDA1 es null - se actualiza
    //                                marcacionAgente.SALIDA1_MOVIL = marcacion.Salida1;
    //                                marcacionAgente.SALIDA1 = marcacion.Salida1;
    //                                marcacionAgente.UBICACION2 = marcacion.Ubicacion;
    //                                _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
    //                                _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"SALIDA1: {marcacionAgente.SALIDA1_MOVIL}, " +
    //                                    $"UBICACION1: {marcacionAgente.UBICACION2}, " +
    //                                    $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
    //                            }
    //                            else
    //                            {
    //                                // SALIDA no es null - no se hace nada
    //                                _logger.LogInformation($"Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"SALIDA 1 ya registrada.");
    //                            }
    //                        }


    //                    }
    //                    await _context.SaveChangesAsync();

    //                }

    //                if (marcacionesE2.Any())
    //                {

    //                    foreach (var marcacion in marcacionesE2)
    //                    {


    //                        var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
    //                            .AsNoTracking()
    //                                            .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
    //                                            && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
    //                        var marcacionAgente = _marcacionAgente.FirstOrDefault();

    //                        if (marcacionAgente != null)
    //                        {
    //                            if (marcacionAgente.ENTRADA2_MOVIL == null)
    //                            {
    //                                // El agente existe y SALIDA1 es null - se actualiza
    //                                marcacionAgente.ENTRADA2_MOVIL = marcacion.Entrada2;
    //                                marcacionAgente.ENTRADA2 = marcacion.Entrada2;
    //                                marcacionAgente.UBICACION3 = marcacion.Ubicacion;
    //                                _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
    //                                _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"ENTRADA2: {marcacionAgente.ENTRADA2_MOVIL}, " +
    //                                    $"UBICACION1: {marcacionAgente.UBICACION3}, " +
    //                                    $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
    //                            }
    //                            else
    //                            {
    //                                // SALIDA no es null - no se hace nada
    //                                _logger.LogInformation($"ENTRADA 2 Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"ENTRADA 2 ya registrada.");
    //                            }
    //                        }


    //                    }
    //                    await _context.SaveChangesAsync();

    //                }

    //                if (marcacionesS2.Any())
    //                {

    //                    foreach (var marcacion in marcacionesS2)
    //                    {


    //                        var _marcacionAgente = await _context.TMP_MARCACION_AGENTE
    //                            .AsNoTracking()
    //                                            .Where(m => m.AGE_CODIGO == Convert.ToDecimal(marcacion.age_codigo)
    //                                            && m.MAR_FECHA.Date == hoy.Date).ToListAsync();
    //                        var marcacionAgente = _marcacionAgente.FirstOrDefault();

    //                        if (marcacionAgente != null)
    //                        {
    //                            if (marcacionAgente.SALIDA2_MOVIL == null)
    //                            {
    //                                // El agente existe y SALIDA1 es null - se actualiza
    //                                marcacionAgente.SALIDA2_MOVIL = marcacion.Salida2;
    //                                marcacionAgente.SALIDA2 = marcacion.Salida2;
    //                                marcacionAgente.UBICACION4 = marcacion.Ubicacion;
    //                                _context.TMP_MARCACION_AGENTE.Update(marcacionAgente);
    //                                _logger.LogInformation($"Actualizado AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"SALIDA2: {marcacionAgente.ENTRADA2_MOVIL}, " +
    //                                    $"UBICACION1: {marcacionAgente.UBICACION4}, " +
    //                                    $"MAR_FECHA: {marcacionAgente.MAR_FECHA}");
    //                            }
    //                            else
    //                            {
    //                                // SALIDA no es null - no se hace nada
    //                                _logger.LogInformation($"SALIDA 2 Sin cambios AGE_CODIGO: {marcacionAgente.AGE_CODIGO}, " +
    //                                    $"SALIDA 2 ya registrada.");
    //                            }
    //                        }


    //                    }
    //                    await _context.SaveChangesAsync();

    //                }

    //            }


    //        }
    //        return marcaciones;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception("Error al obtener las marcaciones de Firebase", ex);

    //    }
    //}




