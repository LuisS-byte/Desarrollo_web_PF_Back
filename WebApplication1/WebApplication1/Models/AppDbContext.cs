using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArchivoTicket> ArchivoTickets { get; set; }

    public virtual DbSet<ComentarioxTicket> ComentarioxTickets { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Prioridad> Prioridads { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketxAsignacion> TicketxAsignacions { get; set; }

    public virtual DbSet<TicketxCambioestado> TicketxCambioestados { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DW;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchivoTicket>(entity =>
        {
            entity.HasKey(e => e.IdArtick).HasName("PK__ARCHIVO___637BBED9079BF1B2");

            entity.ToTable("ARCHIVO_TICKETS");

            entity.Property(e => e.IdArtick).HasColumnName("ID_ARTICK");
            entity.Property(e => e.ArNombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("AR_NOMBRE");
            entity.Property(e => e.ArRuta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("AR_RUTA");
            entity.Property(e => e.IdTicket).HasColumnName("ID_TICKET");
            entity.Property(e => e.TickFechacreacion).HasColumnName("TICK_FECHACREACION");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.ArchivoTickets)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK__ARCHIVO_T__ID_TI__47DBAE45");
        });

        modelBuilder.Entity<ComentarioxTicket>(entity =>
        {
            entity.HasKey(e => e.IdComentario).HasName("PK__COMENTAR__4B0815B1C07845F7");

            entity.ToTable("COMENTARIOxTICKET");

            entity.Property(e => e.IdComentario).HasColumnName("ID_COMENTARIO");
            entity.Property(e => e.ComenDescripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("COMEN_DESCRIPCION");
            entity.Property(e => e.IdTicket).HasColumnName("ID_TICKET");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.ComentarioxTickets)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK__COMENTARI__ID_TI__4E88ABD4");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ComentarioxTickets)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__COMENTARI__ID_US__4F7CD00D");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__ESTADO__241E2E01092E2AC5");

            entity.ToTable("ESTADO");

            entity.Property(e => e.IdEstado).HasColumnName("ID_ESTADO");
            entity.Property(e => e.EstDescripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EST_DESCRIPCION");
            entity.Property(e => e.EstNombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EST_NOMBRE");
        });

        modelBuilder.Entity<Prioridad>(entity =>
        {
            entity.HasKey(e => e.IdPrioridad).HasName("PK__PRIORIDA__295EBCCA71B98D96");

            entity.ToTable("PRIORIDAD");

            entity.Property(e => e.IdPrioridad).HasColumnName("ID_PRIORIDAD");
            entity.Property(e => e.PrioriDescripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PRIORI_DESCRIPCION");
            entity.Property(e => e.PrioriNombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PRIORI_NOMBRE");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__ROL__203B0F6850CCF9F7");

            entity.ToTable("ROL");

            entity.Property(e => e.IdRol).HasColumnName("ID_ROL");
            entity.Property(e => e.RolNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROL_NOMBRE");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK__SERVICIO__C8BDE0EBA633CF5B");

            entity.ToTable("SERVICIO");

            entity.Property(e => e.IdServicio).HasColumnName("ID_SERVICIO");
            entity.Property(e => e.SerDescripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SER_DESCRIPCION");
            entity.Property(e => e.ServNombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SERV_NOMBRE");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTickets).HasName("PK__TICKETS__20129F599444239A");

            entity.ToTable("TICKETS");

            entity.Property(e => e.IdTickets).HasColumnName("ID_TICKETS");
            entity.Property(e => e.IdEstado).HasColumnName("ID_ESTADO");
            entity.Property(e => e.IdPrioridad).HasColumnName("ID_PRIORIDAD");
            entity.Property(e => e.IdServicio).HasColumnName("ID_SERVICIO");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");
            entity.Property(e => e.TickDescripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TICK_DESCRIPCION");
            entity.Property(e => e.TickFechacierre).HasColumnName("TICK_FECHACIERRE");
            entity.Property(e => e.TickFechacreacion).HasColumnName("TICK_FECHACREACION");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("FK__TICKETS__ID_ESTA__44FF419A");

            entity.HasOne(d => d.IdPrioridadNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdPrioridad)
                .HasConstraintName("FK__TICKETS__ID_PRIO__440B1D61");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdServicio)
                .HasConstraintName("FK__TICKETS__ID_SERV__4222D4EF");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__TICKETS__ID_USUA__4316F928");
        });

        modelBuilder.Entity<TicketxAsignacion>(entity =>
        {
            entity.HasKey(e => e.IdAsignacion).HasName("PK__TICKETxA__FDFF35BFE096BA5C");

            entity.ToTable("TICKETxASIGNACION");

            entity.Property(e => e.IdAsignacion).HasColumnName("ID_ASIGNACION");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdTicket).HasColumnName("ID_TICKET");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.TicketxAsignacions)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK__TICKETxAS__ID_TI__4AB81AF0");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TicketxAsignacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__TICKETxAS__ID_US__4BAC3F29");
        });

        modelBuilder.Entity<TicketxCambioestado>(entity =>
        {
            entity.HasKey(e => e.IdComentario).HasName("PK__TICKETxC__4B0815B1B5D3C175");

            entity.ToTable("TICKETxCAMBIOESTADO");

            entity.Property(e => e.IdComentario).HasColumnName("ID_COMENTARIO");
            entity.Property(e => e.EstadoAnterior).HasColumnName("ESTADO_ANTERIOR");
            entity.Property(e => e.EstadoNuevo).HasColumnName("ESTADO_NUEVO");
            entity.Property(e => e.FechaCambio).HasColumnName("FECHA_CAMBIO");
            entity.Property(e => e.IdTicket).HasColumnName("ID_TICKET");
            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");

            entity.HasOne(d => d.EstadoAnteriorNavigation).WithMany(p => p.TicketxCambioestadoEstadoAnteriorNavigations)
                .HasForeignKey(d => d.EstadoAnterior)
                .HasConstraintName("FK__TICKETxCA__ESTAD__5441852A");

            entity.HasOne(d => d.EstadoNuevoNavigation).WithMany(p => p.TicketxCambioestadoEstadoNuevoNavigations)
                .HasForeignKey(d => d.EstadoNuevo)
                .HasConstraintName("FK__TICKETxCA__ESTAD__5535A963");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.TicketxCambioestados)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("FK__TICKETxCA__ID_TI__52593CB8");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TicketxCambioestados)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__TICKETxCA__ID_US__534D60F1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__91136B909A39A1B3");

            entity.ToTable("USUARIO");

            entity.Property(e => e.IdUsuario).HasColumnName("ID_USUARIO");
            entity.Property(e => e.IdRol).HasColumnName("ID_ROL");
            entity.Property(e => e.UsuApellido)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USU_APELLIDO");
            entity.Property(e => e.UsuContraseña)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USU_CONTRASEÑA");
            entity.Property(e => e.UsuCorreo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USU_CORREO");
            entity.Property(e => e.UsuFecharegistro).HasColumnName("USU_FECHAREGISTRO");
            entity.Property(e => e.UsuInterno).HasColumnName("USU_INTERNO");
            entity.Property(e => e.UsuNombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USU_NOMBRE");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__USUARIO__ID_ROL__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
