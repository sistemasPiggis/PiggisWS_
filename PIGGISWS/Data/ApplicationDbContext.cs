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
    public DbSet<DmovInv> DMOVINV { get; set; }

    public DbSet<TipoDev> TIPODEV { get; set; }
    public DbSet<NextVal> NEXVAL { get; set; }

    public DbSet<Rep_Referencias_Dev_Infoa> REP_REFERENCIAS_DEV_INFOA { get; set; }

    public DbSet<Lst_Productos_Apr_Ncc> LST_PRODUCTOS_APR_NCC { get; set; }

    public DbSet<Vl_Ncc_Idc_ldv> VL_NCC_IDC_LDV { get; set; }
    public DbSet<Rep_Ventas_Int_60> REP_VENTAS_INT_60 { get; set; }

    public DbSet<Rep_Motivos_Dev> REP_MOTIVOS_DEV { get; set; }
    public DbSet<Cmovinv> CMOVINV { get; set; }
    public DbSet<Pre_Ventas_Anual> PRE_VENTAS_ANUAL { get; set; }
    public DbSet<Tmp_Marcacion_Agente> TMP_MARCACION_AGENTE { get; set; }

    public DbSet<Devolucion_Cab> DEVOLUCION_CAB { get; set; }
    public DbSet<Devolucion_Det> DEVOLUCION_DET { get; set; }
    public DbSet<Devolucion_Ext> DEVOLUCION_EXT { get; set; }
    public DbSet<Cc_Est_Perididos> CC_EST_PEDIDOS { get; set; }
    public DbSet<Vl_Cc_Est_Pedidosq> VL_CC_EST_PEDIDOSQ { get; set; }
    public DbSet<Rep_Lista_Prod_Ped_Internet> REP_LISTA_PROD_PED_INTERNET { get; set; }
    public DbSet<SaldoCarteraResult> SaldoCarteraResults { get; set; }
    public DbSet<FechaSugResult> FechaSugResults { get; set; }
    public DbSet<Rep_Cantidades_Pedidosa> REP_CANTIDADES_PEDIDOSA { get; set; }
    public DbSet<Rep_Cons_Cartera_Interneta> REP_CONS_CARTERA_INTERNETA { get; set; }
    public DbSet<REP_CANTIDADES_VENTAS_2009> REP_CANTIDADES_VENTAS_2009 { get; set; }
    public DbSet<REP_PEDIDOS_INT_X_DAPP> REP_PEDIDOS_INT_X_DAPP { get; set; }
    public DbSet<Tmp_Dev_Pro_Cli_Age> TMP_DEV_PROD_CLI_AGE { get; set; }
    public DbSet<REP_DET_ESTADO_DESPACHO_PED> REP_DET_ESTADO_DESPACHO_PED { get; set; }
    public DbSet<REP_CART_VEN_INT_T> REP_CART_VEN_INT_T { get; set; }

    public DbSet<CLASIFPROD> CLASIFPROD { get; set; }
    public DbSet<GPRODUCTO> GPRODUCTO { get; set; }

    public DbSet<GEN_MENSAJERIA> GEN_MENSAJERIA { get; set; }
    public DbSet<TDS_PEDIDO_NAV_DET> TDS_PEDIDO_NAV_DET { get; set; }
    public DbSet<TDS_PEDIDOS_NAV_CAB> TDS_PEDIDOS_NAV_CAB { get; set; }

    public decimal F_CXC_SALDO_CARTERA_PED_ST_NR(int empresa, decimal clienteCodigo)
               => throw new NotSupportedException();

    public DateTime F_VNT_FECHA_FACTURAR_DT(int empresa, decimal clienteCodigo, decimal agente)
        => throw new NotSupportedException();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agente>()
            .HasKey(a => new { a.AGE_CODIGO, a.AGE_EMPRESA });
        modelBuilder.Entity<TDS_PEDIDO_NAV_DET>()
            .HasKey(a => new { a.ID_PEDIDO_DET });

        modelBuilder.Entity<TDS_PEDIDOS_NAV_CAB>()
           .HasKey(a => new { a.ID_PEDIDO_NAV });

        modelBuilder.Entity<Tmp_Dev_Pro_Cli_Age>()
           .HasKey(a => new { a.CODIGO_PK });
        modelBuilder.Entity<Clientes_Nuevos>()
            .HasKey(cn => new { cn.ID_SECUENCIA_PK });

        modelBuilder.Entity<GEN_MENSAJERIA>()
           .HasKey(cn => new { cn.ID_MENSAJE });

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


        modelBuilder.Entity<CLASIFPROD>()
          .HasKey(a => new { a.CPR_CODIGO });

        modelBuilder.Entity<GPRODUCTO>()
         .HasKey(a => new { a.GPR_CODIGO });

        #region Vistas
        modelBuilder.Entity<Vw_Insepector_Calidad>().ToTable("VW_INSEPECTOR_CALIDAD").HasKey(c => c.ID); // trae las vistas de la BD

        modelBuilder.Entity<Rep_Cartera_Vencida>().ToTable("REP_CARTERA_VENCIDA").HasKey(c => c.CLI_CODIGO);

        modelBuilder.Entity<Vl_Nomina_Centro_Costo>().ToTable("VL_NOMINA_CENTRO_COSTO").HasKey(c => c.CODIGO_CENTRO_COSTO);

        modelBuilder.Entity<Rep_Referencias_Dev_Infoa>().ToTable("REP_REFERENCIAS_DEV_INFOA").HasKey(c => c.CMO_REFERENCIA);
        modelBuilder.Entity<Vl_Ncc_Idc_ldv>().ToTable("VL_NCC_IDC_LDV").HasKey(a => new { a.ID_EMPRESA_NR, a.TOTAL_NOTA_CREDITO, a.NUMERO_NOTA_CREDITO, a.NUMERO_IDC, a.NUMERO_FACTURA, a.NUMERO_LDV });
        modelBuilder.Entity<Lst_Productos_Apr_Ncc>().ToTable("LST_PRODUCTOS_APR_NCC").HasKey(a => new { a.CCO_EMPRESA, a.CCO_CODIGO, a.ID, a.VALOR });

        modelBuilder.Entity<Rep_Ventas_Int_60>().ToTable("REP_VENTAS_INT_60").HasKey(a => new { a.PRO_CODIGO, a.CCO_CODCLIPRO, a.UMD_ID });
        modelBuilder.Entity<Rep_Motivos_Dev>().ToTable("REP_MOTIVOS_DEV").HasKey(a => new { a.TDE_CODIGO });

        modelBuilder.Entity<Cc_Est_Perididos>().ToTable("CC_EST_PEDIDOS").HasKey(a => new { a.PRO_CODIGO, a.CCO_CODCLIPRO });
        modelBuilder.Entity<Vl_Cc_Est_Pedidosq>().ToTable("VL_CC_EST_PEDIDOSQ").HasKey(a => new { a.PRO_CODIGO, a.CCO_CODCLIPRO });
        modelBuilder.Entity<Rep_Lista_Prod_Ped_Internet>().ToTable("REP_LISTA_PROD_PED_INTERNET").HasKey(a => new { a.PRO_CODIGO, a.DLP_LISTAPRE });
        modelBuilder.Entity<Rep_Cantidades_Pedidosa>().ToTable("REP_CANTIDADES_PEDIDOSA").HasKey(a => new { a.DOC, a.PRO_CODIGO, a.DFAC_CANTIDAD });
        modelBuilder.Entity<Rep_Cons_Cartera_Interneta>().ToTable("REP_CONS_CARTERA_INTERNETA").HasKey(a => new { a.DOC, a.DDO_CODCLIPRO });
        modelBuilder.Entity<REP_CANTIDADES_VENTAS_2009>().ToTable("REP_CANTIDADES_VENTAS_2009").HasKey(a => new { a.DOC, a.PRO_NOMBRE, a.TOTAL_LIBRAS });
        modelBuilder.Entity<REP_PEDIDOS_INT_X_DAPP>().ToTable("REP_PEDIDOS_INT_X_DAPP").HasKey(a => new { a.DOC, a.CCO_AGENTE, a.CANTIDAD_KILOS });
        modelBuilder.Entity<REP_DET_ESTADO_DESPACHO_PED>().ToTable("REP_DET_ESTADO_DESPACHO_PED").HasKey(a => new { a.FAC, a.ESTADO_DESPACHO, a.CLI_NOMBRE });
        modelBuilder.Entity<REP_CART_VEN_INT_T>().ToTable("REP_CART_VEN_INT_T").HasKey(a => new { a.DOC, a.AGE_CODIGO, a.CLI_CLAVE });
        #endregion

       

    modelBuilder.HasDbFunction(typeof(ApplicationDbContext)
                .GetMethod(nameof(F_CXC_SALDO_CARTERA_PED_ST_NR), new[] { typeof(int), typeof(decimal) }))
                .HasName("F_CXC_SALDO_CARTERA_PED_ST_NR");

        modelBuilder.HasDbFunction(typeof(ApplicationDbContext)
            .GetMethod(nameof(F_VNT_FECHA_FACTURAR_DT), new[] { typeof(int), typeof(decimal), typeof(decimal) }))
            .HasName("F_VNT_FECHA_FACTURAR_DT");
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

        modelBuilder.Entity<Tmp_Marcacion_Agente>()
      .HasKey(u => new { u.ID_MARCACION, u.ID_EMPRESA });


        modelBuilder.Entity<DMovInvi>()
         .HasKey(u => new { u.DMO_EMPRESA, u.DMO_CMO_COMPROBA });
        modelBuilder.Entity<DmovInv>()
         .HasKey(u => new { u.DMO_EMPRESA, u.DMO_CMO_COMPROBA });

        modelBuilder.Entity<Devolucion_Cab>()
        .HasKey(u => new { u.DEV_CODIGO });

        modelBuilder.Entity<Devolucion_Det>()
        .HasKey(u => new { u.DVD_CODIGO, u.DVD_PRODUCTO, u.DVD_SECUENCIA });

        modelBuilder.Entity<Devolucion_Cab>()
        .HasKey(u => new { u.DEV_CODIGO });

        modelBuilder.Entity<Devolucion_Ext>()
        .HasKey(u => new { u.DEV_CODIGO, u.DEV_EMPRESA });

        modelBuilder.Entity<Pre_Ventas_Anual>()
           .HasKey(a => new { a.PVA_CODIGO_PK, a.PVA_EMPRESA_FK });

        modelBuilder.Entity<NextVal>()
            .HasKey(u => new { u.NextVl });

        modelBuilder.Entity<SaldoCarteraResult>().HasNoKey();

        modelBuilder.Entity<FechaSugResult>().HasNoKey();
    }


}



