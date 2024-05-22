namespace PIGGISWS.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;


    [Index(nameof(Cli_codigo), nameof(Cli_empresa), IsUnique = true)]
    public class Cliente
    {
   
    public decimal Cli_empresa { get; set; }
        [Key]
        public decimal Cli_codigo { get; set; }

        public string Cli_id { get; set; } = string.Empty;  
        public decimal Cli_tipo { get; set; }
        public string Cli_titulo { get; set; } = string.Empty;
        public string Cli_nombre { get; set; } = string.Empty;
        public string Cli_ruc_cedula { get; set; } = string.Empty;
        public decimal Cli_ciudad { get; set; }
        public decimal Cli_zona { get; set; }
        public string Cli_direccion { get; set; } = string.Empty;
        public string Cli_telefono1 { get; set; } = string.Empty;
        public string Cli_telefono2 { get; set; } = string.Empty;
        public string Cli_telefono3 { get; set; } = string.Empty;
        public string Cli_fax { get; set; } = string.Empty;
        public string Cli_apart_postal { get; set; } = string.Empty;
        public string Cli_mail { get; set; } = string.Empty;
        public decimal Cli_cuenta_deb { get; set; }
        public decimal Cli_cuenta_cre { get; set; }
        public decimal Cli_categoria { get; set; }
        public bool Cli_impuestos { get; set; }
        public decimal Cli_agente { get; set; }
        public string Cli_contacto { get; set; } = string.Empty;
        public string Cli_ref_comercial { get; set; } = string.Empty;
        public string Cli_ref_bancaria { get; set; } = string.Empty;
        public bool Cli_inactivo { get; set; }
        public bool Cli_bloqueo { get; set; }
        public bool Cli_tarjeta { get; set; }
        public decimal Cli_cupo { get; set; }
        public bool Cli_ilimitado { get; set; }
        public decimal Cli_politicas { get; set; }
        public decimal Cli_listapre { get; set; }
        public string Crea_usr { get; set; } = string.Empty;
        public DateTime Crea_fecha { get; set; }
        public string Mod_usr { get; set; } = string.Empty;
        public DateTime Mod_fecha { get; set; }
        public decimal Cli_tipoced { get; set; }
        public decimal Cli_orden { get; set; }
        public decimal Cli_contribuyente { get; set; }
        public decimal Cli_retiva { get; set; }
        public decimal Cli_retfuente { get; set; }
        public DateTime Cli_fechaing { get; set; }
        public decimal Cli_puente { get; set; }
        public decimal Cli_visualiza { get; set; }
        public decimal Cli_icenoiva { get; set; }
        public decimal Cli_tipoper { get; set; }
        public string Cli_nombrecom { get; set; } = string.Empty;
        public decimal Cli_parroquia { get; set; }
        public decimal Cli_canal { get; set; }
        public decimal Cli_tipocli { get; set; }
        public decimal Cli_cuentacla { get; set; }
        public decimal Cli_ruta { get; set; }
        public string Cli_nick { get; set; } = string.Empty;
        public decimal Cli_potencial { get; set; }
        public decimal Cli_establecimiento { get; set; }
        public decimal Cli_nvisita { get; set; }
        public DateTime Cli_hvisita { get; set; }
        public decimal Cli_ubicacion_local { get; set; }
        public decimal Cli_ndias_local { get; set; }
        public decimal Cli_ubicacion_importacion { get; set; }
        public decimal Cli_ndias_importacion { get; set; }
        public decimal Cli_cupo_fact { get; set; }
        public string Cli_diavisita { get; set; } = string.Empty;
        public string Cli_tsujeto { get; set; } = string.Empty;
        public string Cli_sexo { get; set; } = string.Empty;
        public string Cli_estcivil { get; set; } = string.Empty;
        public string Cli_oingresos { get; set; } = string.Empty;
        public string Cli_clave { get; set; } = string.Empty;
        public string Cli_dir_entrega { get; set; } = string.Empty;
        public bool Cli_ref_hipotecaria { get; set; }
        public string Cli_buro { get; set; } = string.Empty;
        public bool Cli_garante { get; set; }
        public string Cli_bienes { get; set; } = string.Empty;
        public string Cli_ref_judiciales { get; set; } = string.Empty;      
        public bool Cli_relacionada { get; set; }
        public decimal Cli_debitob { get; set; }
        public decimal Cli_332 { get; set; }
        public decimal Cli_limitefactura { get; set; }
    
}
