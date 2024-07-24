using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models;

public class Empleado
{
    [Key]
    [Display(Name = "Código de Empleado")]
    public int EMP_CODIGO { get; set; }
    [Display(Name = "Código de Empresa")]
    public int EMP_EMPRESA { get; set;}
    [Display(Name = "ID Empleado")]
    public int EMP_ID { get; set; }
    [Display(Name = "Nombre de Empleado")]
    public string EMP_NOMBRE { get; set; } = string.Empty;
    [Display(Name = "Empleado Estado")]
    public int EMP_INACTIVO { get; set; }
    [Display(Name = "Departamento de Empleado")]
    public int EMP_DEPARTAMENTO { get; set; }
    [Display(Name = "Detalle de Empleado")]
    public int EMP_DETALLE { get; set; }
    [Display(Name = "Cedula o Ruc de Empleado")]
    public string EMP_RUC_CEDULA { get; set; } =string.Empty;
    [Display(Name = "Dirección de Empleado")]
    public string EMP_DIRECCION { get; set; } = string.Empty;
    [Display(Name = "Ciudad de Empleado")]
    public int EMP_CIUDAD { get; set; }
    [Display(Name = "Teléfono de Empleado")]
    public string EMP_TELEFONO1 { get; set; } = string.Empty;
    [Display(Name = "Teléfono 2 de Empleado")]
    public string EMP_TELEFONO2 { get; set; } = string.Empty;
    [Display(Name = "mail de Empleado")]
    public string EMP_MAIL { get; set; } = string.Empty;
    [Display(Name = "Almacen Empleado")]
    public int EMP_ALMACEN { get; set; }
    [Display(Name = "Estado de Empleado")]
    public int EMP_ESTADO { get; set; }

}
