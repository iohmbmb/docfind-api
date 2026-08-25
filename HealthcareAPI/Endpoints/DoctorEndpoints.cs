using HealthcareAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class DoctorEndpoints
{
    public static void MapDoctorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/get/doctor/{id}", (Guid id, AppDbContext context) =>
        {
            var doctor = context.Doctors.First(d => d.Id == id);
            return Results.Ok(doctor);
        }).RequireAuthorization().WithTags("Doctor").WithSummary("Retrieves a doctor.").WithDescription("Retrieves a doctor from the database.");
        
        app.MapGet("/api/get/doctors", async (AppDbContext context) =>
        {
            var doctors = await context.Doctors.ToListAsync();
            return Results.Ok(doctors);
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Patient"}).WithTags("Doctor").WithSummary("Retrieves all doctors.").WithDescription("Retrieves a list of all doctors from the database.");

        app.MapGet("/api/get/doctors/{specialty}", async (PracticeSpecialty specialty, AppDbContext context) =>
        {  
            var users = await context.Doctors.Where(u => u.Role == Users.UserRole.Doctor && u.Specialty == specialty).ToListAsync();
            if (users.Count == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(users);
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Patient"}).WithTags("Doctor").WithSummary("Retrieves doctors by specialty.").WithDescription("Retrieves a list of doctors from the database based on the provided specialty.");
        
        app.MapPut("/api/update/doctor/{id}", async (Guid id, Doctors updatedDoctor, AppDbContext context) =>
        {
            var result = await context.Doctors.FindAsync(id);
            if (result == null)
            {
                return Results.NotFound($"Doctor with ID {id} not found.");
            }
            result.FirstName = updatedDoctor.FirstName;
            result.LastName = updatedDoctor.LastName;
            result.Email = updatedDoctor.Email;
            result.Specialty = updatedDoctor.Specialty;
            result.PracticeName = updatedDoctor.PracticeName;
            result.PracticeAddress = updatedDoctor.PracticeAddress;
            result.PracticeSuburb = updatedDoctor.PracticeSuburb;
            result.PracticeState = updatedDoctor.PracticeState;
            result.PracticePostcode = updatedDoctor.PracticePostcode;
            result.PracticePhone = updatedDoctor.PracticePhone;
            result.Biography = updatedDoctor.Biography;
            result.Preference = updatedDoctor.Preference;
            result.Status = updatedDoctor.Status;
            result.HourlyRate = updatedDoctor.HourlyRate;
            result.ConsultationType = updatedDoctor.ConsultationType;
            
            await context.SaveChangesAsync();
            return Results.Ok();
        }).WithTags("Doctor").WithSummary("Update a doctor.").WithDescription("Update a doctor from the database.");
    }
}