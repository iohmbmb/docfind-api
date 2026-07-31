using System.Security.Claims;
using HealthcareAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/get/users", async (AppDbContext context) =>
        {
            var users = await context.Users.ToListAsync();
            return Results.Ok(users);
        }).WithTags("User").WithSummary("Retrieves all users.").WithDescription("Retrieves a list of all users from the database.");
        
        app.MapGet("/api/get/doctor/{id}", (Guid id, AppDbContext context) =>
        {
            var doctor = context.Doctors.First(d => d.Id == id);
            return Results.Ok(doctor);
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Doctor"}).WithTags("Doctor").WithSummary("Retrieves a doctor.").WithDescription("Retrieves a doctor from the database.");
        
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
            
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization(new AuthorizeAttribute {Roles="Doctor"}).WithTags("Doctor").WithSummary("Update a doctor.").WithDescription("Update a doctor from the database.");
        
        app.MapDelete("/api/delete/user/{id}", async (Guid id, AppDbContext context) =>
        {
            var user = await context.Users.FindAsync(id);
            var doctor = await context.Doctors.FindAsync(id);
            if (user == null && doctor == null)
            {
                return Results.NotFound();
            }
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
            else if (doctor != null)
            {
                context.Doctors.Remove(doctor);
                await context.SaveChangesAsync();
            }
            
            return Results.Ok();
        }).WithTags("User").WithSummary("Deletes a user by ID.").WithDescription("Deletes a user from the database based on the provided ID.");
    }
}