using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Map your Enum to a string so SQLite handles it safely
        modelBuilder.Entity<Appointments>()
            .Property(a => a.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Appointments.AppointmentStatus)Enum.Parse(typeof(Appointments.AppointmentStatus), v ?? ""));
        
        modelBuilder.Entity<Users>()
            .Property(u => u.Role)
            .HasConversion(
                v => v.ToString(),
                v => (Users.UserRole)Enum.Parse(typeof(Users.UserRole), v));
        
        modelBuilder.Entity<Doctors>()
            .Property(u => u.Specialty)
            .HasConversion(
                v => v.ToString(),
                v => (PracticeSpecialty)Enum.Parse(typeof(PracticeSpecialty), v ?? ""));
        
        modelBuilder.Entity<Doctors>()
            .Property(u => u.Preference)
            .HasConversion(
                v => v.ToString(),
                v => (LocationPreference)Enum.Parse(typeof(LocationPreference), v ?? ""));

        modelBuilder.Entity<Doctors>()
            .Property(u => u.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Availability)Enum.Parse(typeof(Availability), v ?? ""));
        
        // 2. Configure Foreign Key relationships.
        // An appointment has ONE patient (User), and a User can have MANY appointments
        modelBuilder.Entity<Appointments>()
            .HasOne<Users>() 
            .WithMany()
            .HasForeignKey(a => a.PatientId) // Make sure this property exists in your Appointments class!
            .OnDelete(DeleteBehavior.Cascade);

        // An appointment has ONE doctor, and a Doctor can have MANY appointments
        modelBuilder.Entity<Appointments>()
            .HasOne<Doctors>()
            .WithMany()
            .HasForeignKey(a => a.DoctorId) // Make sure this property exists in your Appointments class!
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    public DbSet<Users> Users => Set<Users>();
    public DbSet<Doctors> Doctors => Set<Doctors>();
    public DbSet<Appointments> Appointments => Set<Appointments>();
    public DbSet<WaitlistAppointments> WaitlistAppointments => Set<WaitlistAppointments>();
    public DbSet<MedicalRecords> MedicalRecords => Set<MedicalRecords>();
}