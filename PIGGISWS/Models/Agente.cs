using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace PIGGISWS.Models;


public class Agente
{

    public int? AGE_EMPRESA { get; set; }

    public int AGE_CODIGO { get; set; }

    public string? AGE_NOMBRE { get; set; } = string.Empty;

    //public int? AGE_ID { get; set; } 


    public decimal? AGE_EMPLEADO { get; set; }

    public int? AGE_INACTIVO { get; set; }


    //public int? AGE_BODEGA { get; set; }


    //public int? AGE_DEPARTAMENTO { get; set; }

    public decimal? AGE_REPORTA { get; set; }
    public string? AGE_MAIL { get; set; } = string.Empty;
    public int? AGE_ALMACEN { get; set; } 

    //public string? AGE_TELEFONO { get; set; }
}
