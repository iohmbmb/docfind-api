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