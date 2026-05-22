using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Responsavel> Responsaveis => Set<Responsavel>();

    public DbSet<Animal> Animais => Set<Animal>();

    public DbSet<CareEvent> CareEvents => Set<CareEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Responsavel>(entity =>
        {
            entity.ToTable("T_CP_RESPONSAVEIS");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasColumnName("ID");

            entity.Property(t => t.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(t => t.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(t => t.Phone)
                .HasColumnName("PHONE")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(t => t.Cpf)
                .HasColumnName("CPF")
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(t => t.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToTable("T_CP_ANIMAIS");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .HasColumnName("ID");

            entity.Property(p => p.ResponsavelId)
                .HasColumnName("RESPONSAVEL_ID")
                .IsRequired();

            entity.Property(p => p.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(p => p.Nickname)
                .HasColumnName("NICKNAME")
                .HasMaxLength(120);

            entity.Property(p => p.Species)
                .HasColumnName("SPECIES")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(p => p.Breed)
                .HasColumnName("BREED")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(p => p.BirthDate)
                .HasColumnName("BIRTH_DATE")
                .IsRequired();

            entity.Property(p => p.Weight)
                .HasColumnName("WEIGHT")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(p => p.Sex)
                .HasColumnName("SEX")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(p => p.Rga)
                .HasColumnName("RGA")
                .HasMaxLength(30);

            entity.Property(p => p.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(p => p.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();

            // FK: Animal → Responsavel
            entity.HasOne<Responsavel>()
                .WithMany()
                .HasForeignKey(p => p.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CareEvent>(entity =>
        {
            entity.ToTable("T_CP_CARE_EVENTS");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.AnimalId)
                .HasColumnName("ANIMAL_ID")
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasColumnName("TITLE")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(500);

            entity.Property(e => e.ScheduledDate)
                .HasColumnName("SCHEDULED_DATE")
                .IsRequired();

            entity.Property(e => e.CompletedDate)
                .HasColumnName("COMPLETED_DATE");

            entity.Property(e => e.Status)
                .HasColumnName("STATUS")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Priority)
                .HasColumnName("PRIORITY")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("NOTES")
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(e => e.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();

            // FK: CareEvent → Animal
            entity.HasOne<Animal>()
                .WithMany()
                .HasForeignKey(e => e.AnimalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Seed Data ────────────────────────────────────────────────────────

        modelBuilder.Entity<Responsavel>().HasData(
            new Responsavel
            {
                Id        = 1,
                Name      = "Ana Paula Souza",
                Email     = "ana.souza@email.com",
                Phone     = "(11) 91234-5678",
                Cpf       = "123.456.789-00",
                CreatedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                IsActive  = true
            },
            new Responsavel
            {
                Id        = 2,
                Name      = "Carlos Eduardo Lima",
                Email     = "carlos.lima@email.com",
                Phone     = "(21) 99876-5432",
                Cpf       = "987.654.321-00",
                CreatedAt = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                IsActive  = true
            }
        );

        modelBuilder.Entity<Animal>().HasData(
            new Animal
            {
                Id        = 1,
                ResponsavelId   = 1,
                Name      = "Bolinha",
                Nickname  = "Boli",
                Species   = "DOG",
                Breed     = "Labrador",
                BirthDate = new DateTime(2020, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Weight    = 28.5m,
                Sex       = "MALE",
                Rga       = "RGA-001",
                CreatedAt = new DateTime(2025, 1, 11, 0, 0, 0, DateTimeKind.Utc),
                IsActive  = true
            },
            new Animal
            {
                Id        = 2,
                ResponsavelId   = 2,
                Name      = "Mia",
                Nickname  = "Miau",
                Species   = "CAT",
                Breed     = "Siamese",
                BirthDate = new DateTime(2021, 8, 3, 0, 0, 0, DateTimeKind.Utc),
                Weight    = 4.2m,
                Sex       = "FEMALE",
                Rga       = "RGA-002",
                CreatedAt = new DateTime(2025, 2, 16, 0, 0, 0, DateTimeKind.Utc),
                IsActive  = true
            }
        );

        modelBuilder.Entity<CareEvent>().HasData(
            new CareEvent
            {
                Id            = 1,
                AnimalId         = 1,
                Type          = "VACCINE",
                Title         = "Vacina V10 anual",
                Description   = "Aplicação da vacina polivalente V10 anual obrigatória.",
                ScheduledDate = new DateTime(2025, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                CompletedDate = new DateTime(2025, 6, 1, 9, 30, 0, DateTimeKind.Utc),
                Status        = "COMPLETED",
                Priority      = "HIGH",
                Notes         = "Bolinha reagiu bem. Próxima dose em junho de 2026.",
                CreatedAt     = new DateTime(2025, 5, 28, 0, 0, 0, DateTimeKind.Utc),
                IsActive      = true
            },
            new CareEvent
            {
                Id            = 2,
                AnimalId         = 2,
                Type          = "CHECKUP",
                Title         = "Check-up semestral Mia",
                Description   = "Consulta de rotina com exames de sangue e urina.",
                ScheduledDate = new DateTime(2025, 7, 15, 14, 0, 0, DateTimeKind.Utc),
                CompletedDate = null,
                Status        = "PENDING",
                Priority      = "MEDIUM",
                Notes         = "Agendar jejum de 8h antes da consulta.",
                CreatedAt     = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive      = true
            }
        );
    }
}