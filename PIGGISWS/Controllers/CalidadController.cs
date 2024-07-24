using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Newtonsoft.Json;
using PIGGISWS.Data;

using PIGGISWS.Models;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class CalidadController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Bodega bodega = new Bodega();
    DateTime today = DateTime.Today;
    int validador = 0;

    public CalidadController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("getPanificacion")]
    [Authorize]
    public async Task<string> getPanificacion()
    {
        try
        {
            

            var query = await ( from p in _context.PLANIFICA_REVISION
                        join pd in _context.PLANIFICA_REVISION_DET on p.PLR_CODIGO equals pd.PLR_CODIGO
                        join pr in _context.PLANTILLA_REVISION on pd.PLN_CODIGO equals  pr.PLN_CODIGO
                        join e in _context.EMPLEADO on pd.PLD_EMP_ASIGNADO equals e.EMP_CODIGO
                        where pd.PLD_ESTADO ==1 && pd.PLD_FECHA.HasValue ///// quemado en codigo considerar implementar tabla de parametrizacion
                        && pd.PLD_FECHA.Value.Date  == today.Date
                        select new
                        {
                            pr.PLN_NOMBRE,
                            pd.PLD_CODIGO,
                            pd.PLD_FECHA,
                            pd.PLD_FECHA_F,
                            e.EMP_CODIGO,
                            e.EMP_NOMBRE
                        }).ToListAsync();


            model.Status = Response.StatusCode;
            model.Mensaje = "Consulta Realizada Correctamente";
            model.Data = query;
            string resp = JsonConvert.SerializeObject(model);

            return resp;
        }
        catch (Exception ex)
        {
            model.Status = Response.StatusCode;
            model.Mensaje = ex.ToString();
            string respuesta = JsonConvert.SerializeObject(model);

            return respuesta;
        }
    }

    [HttpGet("getInspector")]
    [Authorize]
    public async Task<string> getInspector()
    {
        try
        {


            var query = await (from ins in _context.VW_INSEPECTOR_CALIDAD 
                               select ins).ToListAsync();
            
            model.Status = Response.StatusCode;
            model.Mensaje = "Consulta Realizada Correctamente";
            model.Data = query;
            string resp = JsonConvert.SerializeObject(model);

            return resp;
        }
        catch (Exception ex)
        {
            model.Status = Response.StatusCode;
            model.Mensaje = ex.ToString();
            string respuesta = JsonConvert.SerializeObject(model);

            return respuesta;
        }
    }
    [HttpPut("setPanificacion")]
    [Authorize]
    public async Task<string> setPanificacion(Planifica_Revision_Det prd)
    {
        try
        {

            var entities = _context.PLANIFICA_REVISION_DET
                            .Where(p => p.PLD_CODIGO == prd.PLD_CODIGO)
                            .ToListAsync();

            var entity = entities.Result.FirstOrDefault();




            if (entity != null)
            {
                // Actualizar los campos
                entity.PLD_EMP_ASIGNADO = prd.PLD_EMP_ASIGNADO;
                entity.PLD_FECHA = prd.PLD_FECHA;
                entity.PLD_FECHA_F = prd.PLD_FECHA_F;

                // Guardar los cambios
                validador = await _context.SaveChangesAsync();
                if (validador != 0)
                {

                    model.Mensaje = "Transacción Generada Exitosamente";
                    model.Data = entity;
                    model.Status = Response.StatusCode;
                    return (JsonConvert.SerializeObject(model));


                }
                else
                {

                    model.Mensaje = "No se pudo Guardar verifique que todos los datos estén correctos";
                    model.Data = entity;
                    model.Status = Response.StatusCode;
                    return (JsonConvert.SerializeObject(model));
                }
            }
            else
            {
                
                  
                model.Mensaje = "No se encontró la entidad con el código proporcionado";
                model.Data = prd;
                model.Status = Response.StatusCode;
                return (JsonConvert.SerializeObject(model));
            }
        } 
        catch (Exception ex)
        {
            model.Mensaje = "No se pudo Guardar Los Datos Ingresados" + ex.ToString();
            model.Data = prd;
            model.Status = Response.StatusCode;
            return (JsonConvert.SerializeObject(model));
        }

    }

}
