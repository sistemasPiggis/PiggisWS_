using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PIGGISWS.Models;

public class Bodega
{
    [Key, Column(Order = 1)]
    public int BOD_EMPRESA {  get; set; }
    [Key, Column(Order = 0)]
    public int BOD_CODIGO { get; set; }
    
    public string BOD_ID { get; set; } = string.Empty;
    public string BOD_NOMBRE { get; set; } = string.Empty;
    public int? BOD_CONSIGNA { get; set; }

    public string? BOD_UBICACION { get; set; } = string.Empty;

    public int? BOD_CIUDAD { get; set; } 

    public int? BOD_ZONA {  get; set; }
    public int? BOD_INACTIVO { get; set; }
    public string? BOD_IMPRESORA {  get; set; } = string.Empty;
    
    public int? BOD_LIQUIDACION { get; set; }

    public int? BOD_PROBLEMAS { get; set; }
    public int? BOD_EMPLEADO { get; set; }
    public string? BOD_CUSTODIO { get; set; } = string.Empty ;

    public int? BOD_DIRECTO { get; set; }

    public int? BOD_ALMACEN { get; set; }

    public DateTime? BOD_FECHA_INICIO { get; set; }

    public DateTime? BOD_FECHA_FINAL { get; set; }
    public string? BOD_EMAIL { get; set; } = string.Empty;
   
}
