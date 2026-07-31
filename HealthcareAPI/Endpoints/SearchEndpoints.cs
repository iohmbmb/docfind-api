using HealthcareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search/doctor", async (PracticeSpecialty specialty, float longitude, float latitude, AppDbContext context) =>
        {
            var tolerance = 0.15f;
            var doctors = await context.Doctors.Where(d =>
                d.Latitude >= latitude - tolerance &&
                d.Latitude <= latitude + tolerance &&
                d.Longitude >= longitude - tolerance &&
                d.Longitude <= longitude + tolerance &&
                d.Specialty == specialty).ToListAsync();
            return doctors.Count == 0 ? Results.NotFound() : Results.Ok(doctors);
        });
    }
}