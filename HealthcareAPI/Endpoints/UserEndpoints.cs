using System.Security.Claims;
using HealthcareAPI.Models;
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
        
        app.MapGet("/api/get/doctors", async (AppDbContext context) =>
        {
            var doctors = await context.Doctors.ToListAsync();
            return Results.Ok(doctors);
        }).WithTags("Doctor").WithSummary("Retrieves all doctors.").WithDescription("Retrieves a list of all doctors from the database.");

        app.MapGet("/api/get/doctors/{specialty}", async (string specialty, AppDbContext context) =>
        {  
            var users = await context.Doctors.Where(u => u.Role == Users.UserRole.Doctor && u.Specialty == specialty).ToListAsync();
            if (users.Count == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(users);
        }).WithTags("Doctor").WithSummary("Retrieves doctors by specialty.").WithDescription("Retrieves a list of doctors from the database based on the provided specialty.");
        
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