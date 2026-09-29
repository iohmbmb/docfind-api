using HealthcareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search/doctor", async (PracticeSpecialty specialty, float longitude, float latitude, AppDbContext context) =>
        {
            const float tolerance = 0.15f;
            var workHours = await context.DoctorWorkingHours.ToListAsync();
            var unavailabilities = await context.UnavailabilityPeriod.ToListAsync();
            var doctors = await context.Doctors.Where(d =>
                d.Latitude >= latitude - tolerance &&
                d.Latitude <= latitude + tolerance &&
                d.Longitude >= longitude - tolerance &&
                d.Longitude <= longitude + tolerance &&
                d.Specialty == specialty).ToListAsync();
            
            var today = DateOnly.FromDateTime(DateTime.Now);

            ILookup<Guid, DoctorWorkingHours> hoursLookup = workHours.ToLookup(h => h.DoctorId, h => h);
            ILookup<Guid, UnavailabilityPeriod> unavailabilitiesLookup = unavailabilities.ToLookup(u => u.DoctorId, u => u);

            foreach (var doctor in doctors)
            {
                IEnumerable<DoctorWorkingHours> doctorHours = hoursLookup[doctor.Id].ToList(); 
                IEnumerable<UnavailabilityPeriod> doctorUnavailabilities = unavailabilitiesLookup[doctor.Id].ToList();

                if (!doctorUnavailabilities.Any()) continue;

                foreach (var hour in doctorHours)
                {
                    var targetDayOfWeek = (DayOfWeek)hour.Day; 
                    int daysUntilTarget = ((int)targetDayOfWeek - (int)today.DayOfWeek + 7) % 7;
                    var actualWorkingDate = today.AddDays(daysUntilTarget);

                    bool conflictsWithLeave = doctorUnavailabilities.Any(u => 
                        actualWorkingDate >= u.StartDate && 
                        actualWorkingDate <= u.EndDate);

                    if (conflictsWithLeave)
                    {
                        doctor.IsUnavailable = true;
                        break; 
                    }
                }
            }
            return doctors.Count == 0 ? Results.NoContent() : Results.Ok(doctors);
        });
    }
}