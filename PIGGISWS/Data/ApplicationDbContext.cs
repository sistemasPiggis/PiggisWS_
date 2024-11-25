namespace PIGGISWS.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.CallRecords;
using PIGGISWS.Controllers;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Provincia> PROVINCIA { get; set; }

    public DbSet<Cliente> CLIENTE { get; set; }
    public DbSet<Cliente_dia> CLIENTE_DIA { get; set; }

    public DbSet<Politica> POLITICA { get; set; }
    public DbSet<Menu_Movil> MENU_MOVIL { get; set; }
    public DbSet<Agente> AGENTE { get; set; }
    public DbSet<Promocion_Catalogo> PROMOCION_CATALOGO { get; set; }
    public DbSet<Banco_App> BANCO_APP { get; set; }
    public DbSet<Gen_Noticias> GEN_NOTICIAS { get; set; }

    public DbSet<Bodega> BODEGA { get; set; }
    public DbSet<UsrBod> USRBOD { get; set; }
    public DbSet<Usuario> USUARIO { get; set; }
    public DbSet<Planifica_Revision> PLANIFICA_REVISION { get; set; }

    public DbSet<Planifica_Revision_Det> PLANIFICA_REVISION_DET { get; set; }
    public DbSet<Plantilla_Revision> PLANTILLA_REVISION { get; set; }
    public DbSet<Empleado> EMPLEADO { get; set; }
    public DbSet<Vw_Insepector_Calidad> VW_INSEPECTOR_CALIDAD { get; set; }
    public DbSet<Rep_Cartera_Vencida> REP_CARTERA_VENCIDA { get; set; }
    public DbSet<Vl_Nomina_Centro_Costo> VL_NOMINA_CENTRO_COSTO { get; set; }
    public DbSet<Parametros_Movil> PARAMETROS_MOVIL { get; set; }
    public DbSet<Clientes_Nuevos> CLIENTES_NUEVOS { get; set; }
    public DbSet<Cliente_Dia_Gestion_Nuevo> CLIENTE_DIA_GESTION_NUEVO { get; set; }

    public DbSet<Map_Cerca_Agente> MAP_CERCA_AGENTE { get; set; }
    public DbSet<DFacturai> DFACTURAI { get; set; }

    public DbSet<CComprobai> CCOMPROBAI { get; set; }

    public DbSet<Ccomfaci> CCOMFACI { get; set; }

    public DbSet<UMedida> UMEDIDA { get; set; }

    public DbSet<DListapre> DLISTAPRE { get; set; }
    public DbSet<Producto> PRODUCTO { get; set; }
    public DbSet<CComproba> CCOMPROBA { get; set; }
    public DbSet<CTipocom> CTIPOCOM { get; set; }

    public DbSet<DFactura> DFACTURA { get; set; }
    public DbSet<Provincia_Agente> PROVINCIA_AGENTE { get; set; }

    public DbSet<Ubicacion> UBICACION { get; set; }

    public DbSet<Canton> CANTON { get; set; }

    public DbSet<Zona> ZONA { get; set; }

    public DbSet<TEstableci> TESTABLECI { get; set; }
    public DbSet<ListaPre> LISTAPRE { get; set; }
    public DbSet<Precio_Agente> PRECIO_AGENTE { get; set; }
    public DbSet<Cliente_Ext> CLIENTE_EXT { get; set; }
    public DbSet<Notificaciones> NOTIFICACIONES { get; set; }
    public DbSet<Notificaciones_Grupos> NOTIFICACIONES_GRUPOS { get; set; }
    public DbSet<Fcm_Token> FCM_TOKEN { get; set; }
    public DbSet<Dtipocom> DTIPOCOM { get; set; }
    public DbSet<Sistema> SISTEMA { get; set; }
    public DbSet<Impuesto> IMPUESTO { get; set; }
    public DbSet<Totali> TOTALI { get; set; }
    public DbSet<DListadsc> DLISTADSC { get; set; }
    public DbSet<Rutero> RUTERO { get; set; }
    public DbSet<AgentePedidoCalendario> AGENTE_CALENDARIO_PEDIDO { get; set; }
    public DbSet<Cartera> CARTERA { get; set; }
    public DbSet<DDocumento> DDOCUMENTO { get; set; }

    public DbSet<Total> TOTAL { get; set; }

    public DbSet<DMovInvi> DMOVINVI { get; set; }

    public DbSet<TipoDev> TIPODEV { get; set; }
    public DbSet<NextVal> NEXVAL { get; set; }

    public DbSet<Rep_Referencias_Dev_Info1> REP_REFERENCIAS_DEV_INFO1 { get; set; }  

    public DbSet<Lst_Productos_Apr_Ncc> LST_PRODUCTOS_APR_NCC { get; set; }

    public DbSet<Vl_Ncc_Idc_ldv> VL_NCC_IDC_LDV { get; set; }
    public DbSet<Rep_Ventas_Int_60> REP_VENTAS_INT_60 { get; set; }

    public DbSet<Rep_Motivos_Dev> REP_MOTIVOS_DEV { get; set; }
    public DbSet<Cmovinv> CMOVINV { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agente>()
            .HasKey(a => new { a.AGE_CODIGO, a.AGE_EMPRESA });

        modelBuilder.Entity<Clientes_Nuevos>()
            .HasKey(cn => new { cn.ID_SECUENCIA_PK });


        modelBuilder.Entity<Cliente_Dia_Gestion_Nuevo>()
            .HasKey(cd => new { cd.ID_SECUENCIA_PK });

        modelBuilder.Entity<Bodega>()
            .HasKey(b => new { b.BOD_CODIGO, b.BOD_EMPRESA });

        modelBuilder.Entity<UsrBod>()
           .HasKey(ub => new { ub.UBO_BODEGA, ub.UBO_USUARIO, ub.UBO_EMPRESA });

        modelBuilder.Entity<Cliente_dia>()
          .HasKey(cd => new { cd.CDI_CLIENTE, cd.CDI_DIA, cd.CDI_EMPRESA });

        modelBuilder.Entity<Politica>()
         .HasKey(p => new { p.POL_EMPRESA, p.POL_CODIGO });

        modelBuilder.Entity<Parametros_Movil>()
       .HasKey(pa => new { pa.CODIGO });

        modelBuilder.Entity<Vw_Insepector_Calidad>().ToTable("VW_INSEPECTOR_CALIDAD").HasKey(c => c.ID); // trae las vistas de la BD

        modelBuilder.Entity<Rep_Cartera_Vencida>().ToTable("REP_CARTERA_VENCIDA").HasKey(c => c.CLI_CODIGO);

        modelBuilder.Entity<Vl_Nomina_Centro_Costo>().ToTable("VL_NOMINA_CENTRO_COSTO").HasKey(c => c.CODIGO_CENTRO_COSTO);

        modelBuilder.Entity<Rep_Referencias_Dev_Info1>().ToTable("REP_REFERENCIAS_DEV_INFO1").HasKey(c => c.CMO_REFERENCIA);
        modelBuilder.Entity<Vl_Ncc_Idc_ldv>().ToTable("VL_NCC_IDC_LDV").HasKey(a => new { a.ID_EMPRESA_NR, a.TOTAL_NOTA_CREDITO, a.NUMERO_NOTA_CREDITO, a.NUMERO_IDC, a.NUMERO_FACTURA, a.NUMERO_LDV });
        modelBuilder.Entity<Lst_Productos_Apr_Ncc>().ToTable("LST_PRODUCTOS_APR_NCC").HasKey(a => new { a.CCO_EMPRESA, a.CCO_CODIGO, a.ID, a.VALOR });

        modelBuilder.Entity<Rep_Ventas_Int_60>().ToTable("REP_VENTAS_INT_60").HasKey(a => new { a.PRO_CODIGO, a.CCO_CODCLIPRO, a.UMD_ID });
        modelBuilder.Entity<Rep_Motivos_Dev>().ToTable("REP_MOTIVOS_DEV").HasKey(a => new { a.TDE_CODIGO});

        
        modelBuilder.Entity<Usuario>()
            .HasKey(u => u.USR_CODIGO);

        modelBuilder.Entity<Map_Cerca_Agente>()
            .HasKey(a => new { a.ID_MAP_CERCA });


        modelBuilder.Entity<DFacturai>()
            .HasKey(a => new { a.DFAC_SECUENCIA, a.DFAC_CFAC_COMPROBA, a.DFAC_EMPRESA });
        modelBuilder.Entity<CComprobai>()
            .HasKey(a => new { a.CCO_CODIGO, a.CCO_EMPRESA });

        modelBuilder.Entity<Ccomfaci>()
            .HasKey(a => new { a.CFAC_CCO_COMPROBA, a.CFAC_EMPRESA });


        modelBuilder.Entity<UMedida>()
           .HasKey(a => new { a.UMD_CODIGO, a.UMD_EMPRESA });

        modelBuilder.Entity<DListapre>()
           .HasKey(a => new { a.DLP_CODIGO, a.DLP_LISTAPRE, a.DLP_EMPRESA });

        modelBuilder.Entity<Producto>()
         .HasKey(a => new { a.PRO_CODIGO, a.PRO_EMPRESA });

        modelBuilder.Entity<CComproba>()
           .HasKey(a => new { a.CCO_EMPRESA, a.CCO_CODIGO });

        modelBuilder.Entity<CTipocom>()
          .HasKey(a => new { a.CTI_CODIGO, a.CTI_EMPRESA });
        modelBuilder.Entity<DFactura>()
      .HasKey(a => new { a.DFAC_CFAC_COMPROBA, a.DFAC_EMPRESA, a.DFAC_SECUENCIA });

        modelBuilder.Entity<Provincia_Agente>()
            .HasKey(pa => new { pa.ID_SECUENCIA_PK });

        modelBuilder.Entity<Ubicacion>()
          .HasKey(u => new { u.UBI_CODIGO, u.UBI_EMPRESA });
        modelBuilder.Entity<Canton>()
         .HasKey(u => new { u.ID_CANTON_PK });

        modelBuilder.Entity<Zona>()
        .HasKey(u => new { u.ZON_CODIGO, u.ZON_EMPRESA });


        modelBuilder.Entity<TEstableci>()
       .HasKey(u => new { u.TES_CODIGO, u.TES_EMPRESA });

        modelBuilder.Entity<ListaPre>()
       .HasKey(u => new { u.LPR_CODIGO, u.LPR_EMPRESA });

        modelBuilder.Entity<Precio_Agente>()
      .HasKey(u => new { u.ID_EMPRESA_FK, u.ID_PRECIO_AGENTE_PK });

        modelBuilder.Entity<Cliente_Ext>()
      .HasKey(u => new { u.CLI_CODIGO, u.CLI_EMPRESA });


        modelBuilder.Entity<Notificaciones>()
     .HasKey(u => new { u.NOT_EMPRESA, u.NOT_CODIGO });

        modelBuilder.Entity<Notificaciones_Grupos>()
     .HasKey(u => new { u.NOT_EMPRESA, u.NOT_NOT_CODIGO });

        modelBuilder.Entity<Fcm_Token>()
     .HasKey(u => new { u.FCM_CODIGO });

        modelBuilder.Entity<Dtipocom>()
    .HasKey(u => new { u.DTI_EMPRESA, u.DTI_CTI_CODIGO });


        modelBuilder.Entity<Sistema>()
    .HasKey(u => new { u.SIS_CODIGO });

        modelBuilder.Entity<Impuesto>()
    .HasKey(u => new { u.IMP_CODIGO });

        modelBuilder.Entity<Totali>()
  .HasKey(u => new { u.TOT_CCO_COMPROBA });

        modelBuilder.Entity<DListadsc>()
                    .HasKey(u => new { u.DLD_CODIGO });

        modelBuilder.Entity<Rutero>()
                    .HasKey(u => new { u.RUT_EMPRESA, u.RUT_CLIENTE, u.RUT_AGENTE, u.RUT_FECHA });

        modelBuilder.Entity<Cmovinv>()
                    .HasKey(u => new { u.CMO_EMPRESA, u.CMO_CCO_COMPROBA });

        modelBuilder.Entity<Total>()
                    .HasKey(u => new { u.TOT_EMPRESA, u.TOT_CCO_COMPROBA });


        modelBuilder.Entity<AgentePedidoCalendario>()
        .HasKey(u => new { u.AGE_ID_CALENDARIO_PK });

        modelBuilder.Entity<Cartera>()
        .HasKey(u => new { u.CRT_PAGO, u.CRT_DOCTRAN, u.CRT_TRANSACC, u.CRT_DDO_COMPROBA, u.CRT_FECHA, u.CRT_EMPRESA });


        modelBuilder.Entity<DDocumento>()
       .HasKey(u => new { u.DDO_PAGO, u.DDO_DOCTRAN, u.DDO_TRANSACC, u.DDO_CCO_COMPROBA, u.DDO_EMPRESA });

        modelBuilder.Entity<TipoDev>()
      .HasKey(u => new { u.TDE_EMPRESA, u.TDE_CODIGO });


        modelBuilder.Entity<DMovInvi>()
     .HasKey(u => new { u.DMO_EMPRESA, u.DMO_CMO_COMPROBA });


        modelBuilder.Entity<NextVal>()
 .HasKey(u => new { u.NextVl });


    }
}



