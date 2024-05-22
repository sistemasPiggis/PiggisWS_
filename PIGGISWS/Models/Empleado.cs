using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Empleado
{
    [Key]
    public int EMP_CODIGO { get; set; }
    public int EMP_EMPRESA { get; set;}
    public int EMP_ID { get; set; }
    public string EMP_NOMBRE { get; set; } = string.Empty;
    public int EMP_INACTIVO { get; set; }
    public int EMP_DEPARTAMENTO { get; set; }
    public int EMP_DETALLE { get; set; }
    public string EMP_RUC_CEDULA { get; set; } =string.Empty;
    public string EMP_DIRECCION { get; set; } = string.Empty;
    public int EMP_CIUDAD { get; set; }
    public string EMP_TELEFONO1 { get; set; } = string.Empty;
    public string EMP_TELEFONO2 { get; set; } = string.Empty;
    public string EMP_MAIL { get; set; } = string.Empty;
    public int EMP_ALMACEN { get; set; } 
    public int EMP_ESTADO { get; set; }




}
