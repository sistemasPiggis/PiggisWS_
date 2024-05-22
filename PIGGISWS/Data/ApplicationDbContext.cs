namespace PIGGISWS.Data;

using Microsoft.EntityFrameworkCore;
using PIGGISWS.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Provincia> PROVINCIA { get; set; }

    public DbSet<Cliente> CLIENTE { get; set; }

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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agente>()
            .HasKey(a => new { a.AGE_CODIGO, a.AGE_EMPRESA });

        modelBuilder.Entity<Bodega>()
            .HasKey(b => new { b.BOD_CODIGO, b.BOD_EMPRESA });

        modelBuilder.Entity<UsrBod>()
           .HasKey(ub => new { ub.UBO_BODEGA, ub.UBO_USUARIO, ub.UBO_EMPRESA });

        //modelBuilder.Entity<UsrBod>()
        //    .HasOne(ub => ub.USUARIO)
        //    .WithMany(us => us.USERBODS)
        //    .HasForeignKey(us => us.UBO_USUARIO);

        //modelBuilder.Entity<UsrBod>()
        //    .HasOne(ub => ub.BODEGA)
        //    .WithMany(b => b.UsrBods)
        //    .HasForeignKey(ub => new { ub.UBO_BODEGA, ub.UBO_EMPRESA });


        modelBuilder.Entity<Usuario>()
            .HasKey(u => u.USR_CODIGO);

    }
}
