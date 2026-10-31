using LifePet.Domain.Entities;
using LifePet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LifePet.Infrastructure.Persistence;

public class LifePetDbContext(DbContextOptions<LifePetDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Historico> Historicos => Set<Historico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.ToTable("TUTORES");
            entity.HasKey(tutor => tutor.Id);
            entity.Property(tutor => tutor.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(tutor => tutor.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(tutor => tutor.Email).HasColumnName("EMAIL").HasMaxLength(150).IsRequired();
            entity.Property(tutor => tutor.Telefone).HasColumnName("TELEFONE").HasMaxLength(20);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("PETS");
            entity.HasKey(pet => pet.Id);
            entity.Property(pet => pet.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(pet => pet.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(pet => pet.Especie).HasColumnName("ESPECIE").HasMaxLength(50);
            entity.Property(pet => pet.Raca).HasColumnName("RACA").HasMaxLength(50);
            entity.Property(pet => pet.DataNascimento).HasColumnName("DATA_NASCIMENTO");
            entity.Property(pet => pet.TutorId).HasColumnName("TUTOR_ID");
            entity.HasOne(pet => pet.Tutor).WithMany(tutor => tutor.Pets).HasForeignKey(pet => pet.TutorId);
        });

        modelBuilder.Entity<Vacina>(entity =>
        {
            entity.ToTable("VACINAS");
            entity.HasKey(vacina => vacina.Id);
            entity.Property(vacina => vacina.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(vacina => vacina.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(vacina => vacina.DataAplicacao).HasColumnName("DATA_APLICACAO");
            entity.Property(vacina => vacina.DataProximaDose).HasColumnName("DATA_PROXIMA_DOSE");
            entity.Property(vacina => vacina.PetId).HasColumnName("PET_ID");
            entity.HasOne(vacina => vacina.Pet).WithMany(pet => pet.Vacinas).HasForeignKey(vacina => vacina.PetId);
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("CONSULTAS");
            entity.HasKey(consulta => consulta.Id);
            entity.Property(consulta => consulta.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(consulta => consulta.DataHora).HasColumnName("DATA_HORA");
            entity.Property(consulta => consulta.Veterinario).HasColumnName("VETERINARIO").HasMaxLength(100);
            entity.Property(consulta => consulta.Motivo).HasColumnName("MOTIVO").HasMaxLength(200);
            entity.Property(consulta => consulta.PetId).HasColumnName("PET_ID");
            entity.HasOne(consulta => consulta.Pet).WithMany(pet => pet.Consultas).HasForeignKey(consulta => consulta.PetId);
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.ToTable("MEDICAMENTOS");
            entity.HasKey(medicamento => medicamento.Id);
            entity.Property(medicamento => medicamento.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(medicamento => medicamento.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
            entity.Property(medicamento => medicamento.Dosagem).HasColumnName("DOSAGEM").HasMaxLength(50);
            entity.Property(medicamento => medicamento.Frequencia).HasColumnName("FREQUENCIA").HasMaxLength(50);
            entity.Property(medicamento => medicamento.DataInicio).HasColumnName("DATA_INICIO");
            entity.Property(medicamento => medicamento.DataFim).HasColumnName("DATA_FIM");
            entity.Property(medicamento => medicamento.PetId).HasColumnName("PET_ID");
            entity.HasOne(medicamento => medicamento.Pet).WithMany(pet => pet.Medicamentos).HasForeignKey(medicamento => medicamento.PetId);
        });

        modelBuilder.Entity<Historico>(entity =>
        {
            entity.ToTable("HISTORICOS");
            entity.HasKey(historico => historico.Id);
            entity.Property(historico => historico.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(historico => historico.Data).HasColumnName("DATA");
            entity.Property(historico => historico.Descricao).HasColumnName("DESCRICAO").HasMaxLength(500);
            entity.Property(historico => historico.Tipo).HasColumnName("TIPO").HasMaxLength(50);
            entity.Property(historico => historico.PetId).HasColumnName("PET_ID");
            entity.HasOne(historico => historico.Pet).WithMany(pet => pet.Historicos).HasForeignKey(historico => historico.PetId);
        });
    }
}
