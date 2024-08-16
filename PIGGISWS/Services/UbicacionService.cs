using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using System.Text.Json;

namespace PIGGISWS.Services;

public class UbicacionService :IUbicacionService
{
    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa = 0;
    int p_ap_Inactivo = 0;
    int p_ca_Inactivo = 0;
    int p_es_Inactivo = 0;
    int p_es_Umedida = 0;
    int p_es_Length_Id = 0;
    List<int> provincias = new List<int>();
    public UbicacionService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();

    }

    public void GetParametros()
    {
        parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "UbicacionService" || p.SERVICIO == "GENERAL").ToList();
        p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
        p_ap_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 9)?.VALOR ?? "0");
        p_ca_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 10)?.VALOR ?? "0");
        p_es_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p=>p.CODIGO == 11)?.VALOR ?? "0");
        
        p_es_Length_Id = Convert.ToInt32(parametros.FirstOrDefault(p =>p.CODIGO == 13)?.VALOR ?? "0");
        p_es_Umedida = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 12)?.VALOR ?? "0");
    }

    public async Task<ServiceResponse<object>> GetProvinciasxAgente(int agente)
    {
        var response = new ServiceResponse<object>();


        try
        {

            


            var provincias = await (from p in _context.PROVINCIA
                                   join pa in _context.PROVINCIA_AGENTE on p.ID_PROVINCIA_PK equals pa.ID_PROVINCIA_FK
                                   where pa.ID_AGENTE_FK == agente 
                                   && pa.ID_EMPRESA_FK == p_empresa 
                                   && pa.INACTIVO_NR == p_ap_Inactivo
                                   select new
                                   {
                                       p.ID_PROVINCIA_PK,
                                       p.NOMBRE_TX
                                   }).ToListAsync();

                                   

            if (provincias == null || !provincias.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "No se encontraron Provincias.";

            }

            response.Data = provincias;
            response.Success = true;
            response.Message = "Provincias encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
           
            response.Success = false;
            response.Message = "Ocurrió un error al obtener las provincias.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;


    }


    public async Task<ServiceResponse<object>> GetCiudadesxProvincia(List<Provincia> provincia)
    {
        var response = new ServiceResponse<object>();
        var idsProvincias = provincia.Select(c => c.ID_PROVINCIA_PK).ToList();

        try
        {
            var cantones = await (from c in _context.CANTON
                                 
                                    where idsProvincias.Contains(c.ID_PROVINCIA_FK)
                                    && c.INACTIVO_NR == p_ca_Inactivo
                                    select new
                                    {
                                        c.ID_CANTON_PK,
                                        c.NOMBRE_TX,
                                        c.ID_PROVINCIA_FK
                                    }).ToListAsync();



            if (cantones == null || !cantones.Any())
            {
                response.Data = cantones;
                response.Success = true;
                response.Message = "No se encontraron Cantones.";

            }

            response.Data = cantones;
            response.Success = true;
            response.Message = "Cantones encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener las Cantones.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;


    }

    public async Task<ServiceResponse<object>> GetParroquiasxCanton(List<Canton> cantones)
    {
       
            var response = new ServiceResponse<object>();
        var idsCantones = cantones.Select(c => (int?)c.ID_CANTON_PK).ToList();


        try
            {
                var parroquias = await (from u in _context.UBICACION

                                      where idsCantones.Contains(u.UBI_REPORTA)
                                      && u.UBI_INACTIVO == p_ca_Inactivo
                                      select new
                                      {
                                         u.UBI_REPORTA,
                                         u.UBI_CODIGO,
                                         u.UBI_INACTIVO,
                                         u.UBI_ORDEN
                                      }).ToListAsync();



                if (parroquias == null || !parroquias.Any())
                {
                    response.Data = parroquias;
                    response.Success = true;
                    response.Message = "No se encontraron Parroquias.";

                }

                response.Data = parroquias;
                response.Success = true;
                response.Message = "Parroquias encontrados exitosamente.";
            }
            catch (NotFoundException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = "Ocurrió un error al obtener las Parroquias.";
                throw new DatabaseException("Error de base de datos.", ex);
            }

            return response;

        }

    public async Task<ServiceResponse<object>> GetZonasxCanton(List<Canton> cantones)
    {

        var response = new ServiceResponse<object>();
        var idsCantones = cantones.Select(c => (int?)c.ID_CANTON_PK).ToList();

        try
        {
            var query1 = await( from z in _context.ZONA
                          where z.ZON_EMPRESA == p_empresa
                          && idsCantones.Contains(z.ZON_CANTON_FK)
                          && z.ZON_ID.Length >= 4
                          && z.ZON_CANTON_FK != null
                          select new 
                          {
                              Nombre = z.ZON_NOMBRE,
                              Codigo = z.ZON_CODIGO
                          }).ToListAsync();

            
            var query2 = await (from z in _context.ZONA
                         where z.ZON_EMPRESA == 1
                         && z.ZON_CANTON_FK == null
                         && z.ZON_ID == "0000"
                         select new 
                         {
                             Nombre = z.ZON_NOMBRE,
                             Codigo = z.ZON_CODIGO
                         }).ToListAsync();

            // Unión de ambas consultas
            var zona = query1.Union(query2).ToList();


            if (zona == null || !zona.Any())
            {
                response.Data = zona;
                response.Success = true;
                response.Message = "No se encontraron Parroquias.";

            }

            response.Data = zona;
            response.Success = true;
            response.Message = "Parroquias encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener las Parroquias.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;

    }



    public async Task<ServiceResponse<object>> GetEstablecimientos()
    {

        var response = new ServiceResponse<object>();
        

        try
        {
            var establecimientos = await (from e in _context.TESTABLECI
                                where e.TES_EMPRESA == p_empresa
                                && e.TES_INACTIVO == p_ap_Inactivo
                                && e.TES_ID.Length >= p_es_Length_Id
                                && e.TES_UMEDIDA == p_es_Umedida
                                select e).ToListAsync();


           


            if (establecimientos == null || !establecimientos.Any())
            {
                response.Data = establecimientos;
                response.Success = true;
                response.Message = "No se encontraron Establecimientos.";

            }

            response.Data = establecimientos;
            response.Success = true;
            response.Message = "Parroquias encontrados Establecimientos.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener las Establecimientos.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;

    }

    /// <summary>
    /// Envia los datos de provincia, canton, ubicacion (parroquias)
    /// </summary>
    /// <param name="agente"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseException"></exception>
    public async Task<ServiceResponse<object>> GetUbicacionxAgenteAsync(int agente)
    {
        var response = new ServiceResponse<object>();


        try
        {





            var provinciasQuery = await (from p in _context.PROVINCIA
                                         join pa in _context.PROVINCIA_AGENTE on p.ID_PROVINCIA_PK equals pa.ID_PROVINCIA_FK
                                         join c in _context.CANTON on p.ID_PROVINCIA_PK equals c.ID_PROVINCIA_FK
                                         join pr in _context.UBICACION on c.ID_CANTON_PK equals pr.UBI_REPORTA
                                         where pa.ID_AGENTE_FK == agente
                                            && pa.ID_EMPRESA_FK == p_empresa
                                            && pa.INACTIVO_NR == p_ap_Inactivo
                                         select new { Provincia = p, Canton = c, Ubicacion = pr })
                                .ToListAsync();

            var resultado = new AuxUbicacion
            {
                Data = provinciasQuery
                    .GroupBy(x => x.Provincia)
                    .Select(grpProvincia => new DataItem
                    {
                        Provincia = grpProvincia.Key,
                        Cantones = grpProvincia
                            .GroupBy(x => x.Canton)
                            .Select(grpCanton => new CantonItem
                            {
                                Canton = grpCanton.Key,
                                Ubicaciones = grpCanton.Select(x => x.Ubicacion).Distinct().ToList()
                            }).ToList()
                    }).ToList()
            };
            

            if (resultado == null )
            {
                response.Data = null;
                response.Success = true;
                response.Message = "No se encontraron Provincias.";

            }

            response.Data = resultado;
            response.Success = true;
            response.Message = "Provincias encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener las provincias.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

   


}
