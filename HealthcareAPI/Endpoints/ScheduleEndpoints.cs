using HealthcareAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class ScheduleEndpoints
{
    public static void MapScheduleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedule/get/{id}/absence", (Guid id, AppDbContext context) =>
            {
                var absencePeriod = context.UnavailabilityPeriod.FirstOrDefault(u => u.DoctorId == id);
                return absencePeriod == null ? Results.NotFound() : Results.Ok(absencePeriod);
            }).RequireAuthorization(new AuthorizeAttribute {Roles="Doctor"})
            .WithTags("Schedule")
            .WithSummary("Retrieves absence periods.")
            .WithDescription("This endpoint retrieves the absence periods for a specific doctor.");
        
        app.MapGet("/api/schedule/get/{id}/workhours", async (Guid id, AppDbContext context) =>
            {
                var hours = await context.DoctorWorkingHours
                    .Where(x => x.DoctorId == id)
                    .ToListAsync();
                var orderedHours = hours.OrderBy(d => d.Day == DayOfWeek.Sunday ? 7 : (int)d.Day).ToList();
                return hours.Count > 0 ? Results.Ok(orderedHours) : Results.NotFound();
            }).WithTags("Schedule")
            .WithSummary("Retrieves work hours.")
            .WithDescription("This endpoint retrieves the work hours for a specific doctor.");
        
        app.MapPost("/api/schedule/post/{id}/workhours", async (Guid id, List<DoctorWorkingHours> hours, AppDbContext context) =>
            {
                var existingHours = await context.DoctorWorkingHours
                    .Where(x => x.DoctorId == id)
                    .ToListAsync();
                
                context.DoctorWorkingHours.RemoveRange(existingHours);

                hours.ForEach(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.DoctorId = id;
                });
                
                context.DoctorWorkingHours.AddRange(hours);
                
                await context.SaveChangesAsync();
                return Results.Created($"/api/schedule/post/{id}/workhours", hours);
            }).RequireAuthorization()
            .WithTags("Schedule")
            .WithSummary("Create/Update work hours.")
            .WithDescription("This endpoint create or update the work hours for a specific doctor.");

        app.MapPost("/api/schedule/post/{docId}/absence", async (Guid docId, UnavailabilityPeriod period, AppDbContext context) =>
        {
            var existing = await context.UnavailabilityPeriod.FirstOrDefaultAsync(x => x.DoctorId == docId);

            if (existing == null)
            {
                period.Id = Guid.NewGuid();
                period.DoctorId = docId;
                context.UnavailabilityPeriod.Add(period);
            }
            else
            {
                existing.StartDate = period.StartDate;
                existing.EndDate = period.EndDate;
            }
            await context.SaveChangesAsync();
            return Results.Ok(); 
        }).RequireAuthorization(new AuthorizeAttribute {Roles="Doctor"})
        .WithTags("Schedule")
        .WithSummary("Create absence period.")
        .WithDescription("This endpoint create or update the absence periods for a specific doctor.");
    }
}