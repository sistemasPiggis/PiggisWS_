namespace PIGGISWS.Data;

using Microsoft.EntityFrameworkCore;
using PIGGISWS.Models;
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
    public DbSet <Usuario> USUARIO { get; set; }
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
       .HasKey(pa => new { pa.CODIGO});

        modelBuilder.Entity<Vw_Insepector_Calidad>().ToTable("VW_INSEPECTOR_CALIDAD").HasKey(c => c.ID); // trae las vistas de la BD

        modelBuilder.Entity<Rep_Cartera_Vencida>().ToTable("REP_CARTERA_VENCIDA").HasKey(c => c.CLI_CODIGO);

        modelBuilder.Entity<Vl_Nomina_Centro_Costo>().ToTable("VL_NOMINA_CENTRO_COSTO").HasKey(c => c.CODIGO_CENTRO_COSTO);


        modelBuilder.Entity<Usuario>()
            .HasKey(u => u.USR_CODIGO);

        modelBuilder.Entity<Map_Cerca_Agente>()
            .HasKey(a => new { a.ID_MAP_CERCA });

    }
}
