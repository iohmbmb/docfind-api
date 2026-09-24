
using Bogus;
using HealthcareAPI.Models;

namespace HealthcareAPI.DataSeeders;
public static class MedicalDataSeeder
{
    public static void SeedMedicalSystem(AppDbContext context)
    {
        var unavailableMocks = context.UnavailabilityPeriod.Where(u => u.IsMock == true).ToList();
        var workHoursMock = context.DoctorWorkingHours.Where(w => w.IsMock == true).ToList();
        var docMocks = context.Doctors.Where(d => d.IsMock == true).ToList();
        context.Doctors.RemoveRange(docMocks);
        context.DoctorWorkingHours.RemoveRange(workHoursMock);
        context.UnavailabilityPeriod.RemoveRange(unavailableMocks);
        
        context.SaveChanges();
        
        var newPatientOptions = new[] { "Initial Consultation", "Comprehensive Intake", "First-time Assessment", "New Patient Screening" };
        var existingPatientOptions = new[] { "Routine Follow-up", "Prescription Refill Visit", "Chronic Care Review", "Results Discussion" };
        
        var ivryPriorityAnchor = (Suburb:"Ivry-sur-Seine", Postcode:"94200", Lat:48.8130f, Lng:2.3882f);
        var alternativeAnchors = new[]
        {
            (Suburb: "Paris", Postcode: "75001", Lat:48.8566f, Lng:2.3522f),
            (Suburb:"Vitry-sur-Seine", Postcode:"94400", Lat:48.7875f, Lng:2.3928f),
            (Suburb:"Villejuif", Postcode:"94800", Lat:48.7900f, Lng:2.3600f),
            (Suburb:"Charenton-le-Pont", Postcode:"94220", Lat:48.8211f, Lng:2.4144f)
        };

        var doctorFaker = new Faker<Doctors>("fr")
            .RuleFor(d => d.Id, f => Guid.NewGuid())
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .RuleFor(d => d.Specialty,
                f => f.PickRandom(new[]
                {
                    PracticeSpecialty.Physiotherapy, PracticeSpecialty.GeneralPractice, PracticeSpecialty.Dietitian,
                    PracticeSpecialty.Podiatry, PracticeSpecialty.Cardiology, PracticeSpecialty.Chiropractor
                }))
            .RuleFor(d => d.Email, (f, d) => f.Internet.Email(d.FirstName, d.LastName))
            .RuleFor(d => d.HourlyRate, f => f.PickRandom(new[] { 15f, 20f, 25f, 30f, 35f, 40f }))
            .RuleFor(d => d.Role, f => Users.UserRole.Doctor)
            .RuleFor(d => d.Preference,
                f => f.PickRandom(new[]
                    { LocationPreference.Hybrid, LocationPreference.Remote, LocationPreference.InPerson }))
            .RuleFor(d => d.Status,
                f => f.PickRandom(new[] { Availability.Available, Availability.Away, Availability.Busy }))
            .RuleFor(d => d.ConsultationType,
                f => new ConsultationType() { New = newPatientOptions, Existing = existingPatientOptions })
            .RuleFor(d => d.ImagePath, f => f.Internet.Avatar())
            .RuleFor(d => d.PracticePhone, f => f.Phone.PhoneNumber("01 ## ## ## ##"))
            .RuleFor(d => d.Biography, f => f.Lorem.Paragraph(10))
            .FinishWith((f, d) =>
            {
                // 60% probability of choosing Ivry-sur-Seine, 40% choosing the other 4 local areas
                var chosenAnchor = f.Random.Bool(0.60f) 
                    ? ivryPriorityAnchor 
                    : f.PickRandom(alternativeAnchors);

                // Tiny structural offset (approx 50 to 200 meters) so pins don't overlap exactly
                float latOffset = (float)f.Random.Double(-0.002, 0.002);
                float lngOffset = (float)f.Random.Double(-0.003, 0.003);

                d.Latitude = chosenAnchor.Lat + latOffset;
                d.Longitude = chosenAnchor.Lng + lngOffset;
        
                d.PracticeName = $"Cabinet du Dr. {d.LastName}";
                d.PracticeAddress = $"{f.Address.BuildingNumber()} {f.Address.StreetName()}";
                d.PracticeSuburb = chosenAnchor.Suburb;
                d.PracticeState = "Île-de-France";
                d.PracticePostcode = chosenAnchor.Postcode;

                d.FirstName = $"Dr. {d.FirstName}";
                d.IsMock = true;
            });

        List<Doctors> doctors = doctorFaker.Generate(15);
        context.Doctors.AddRange(doctors);

        var workDays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
        var workHourFaker = new Faker<DoctorWorkingHours>()
            .RuleFor(w => w.Id, f => Guid.NewGuid())
            .RuleFor(w => w.DoctorId, f => f.PickRandom(doctors).Id)
            .RuleFor(w => w.Day, f => f.PickRandom(workDays))
            .RuleFor(w => w.StartTime,
                f => TimeOnly.FromTimeSpan(f.PickRandom(new[] { TimeSpan.FromHours(8), TimeSpan.FromHours(9) })))
            .RuleFor(w => w.EndTime,
                f => TimeOnly.FromTimeSpan(f.PickRandom(new[] { TimeSpan.FromHours(16), TimeSpan.FromHours(17) })))
            .FinishWith((f, w) => w.IsMock = true);

        List<DoctorWorkingHours> workingHoursList = workHourFaker.Generate(40);
        context.DoctorWorkingHours.AddRange(workingHoursList);

        var unavailabilityFaker = new Faker<UnavailabilityPeriod>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.DoctorId, f => f.PickRandom(doctors).Id)
            .RuleFor(u => u.StartDate, f => DateOnly.FromDateTime(f.Date.Soon(30)))
            .RuleFor(u => u.EndDate, (f, u) => u.StartDate.AddDays(f.Random.Number(1, 5)))
            .FinishWith((f, u) => u.IsMock = true);
        
        List<UnavailabilityPeriod> unavailabilityPeriods = unavailabilityFaker.Generate(10);
        context.UnavailabilityPeriod.AddRange(unavailabilityPeriods);

        context.SaveChanges();
        Console.WriteLine("Old medical seeds cleared and fresh ones successfully generated!");
    }
}
