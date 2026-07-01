using HealthcareAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Endpoints;

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/get/user/{id}/appointments", async (Guid id, AppDbContext context) =>
        {
            var appointments = await context.Appointments.Where(a => a.PatientId == id || a.DoctorId == id).ToListAsync();
            return Results.Ok(appointments);
        }).RequireAuthorization().WithTags("Appointment").WithSummary("Retrieves appointments for a user by ID.").WithDescription("Retrieves a list of appointments from the database for the specified user based on their ID.");
        
        app.MapGet("/api/get/record/{appointmentId}", async (Guid appointmentId, AppDbContext context) =>
        {
            var records = await context.MedicalRecords.Where(r => r.AppointmentId == appointmentId).ToListAsync();
            return Results.Ok(records);
        }).RequireAuthorization().WithTags("MedicalRecord").WithSummary("Retrieves medical records for an appointment by ID.").WithDescription("Retrieves a list of medical records from the database for the specified appointment based on its ID.");
        
        app.MapPost("/api/create/appointment", async (Appointments appointments, AppDbContext context) =>
        {
            appointments.Id = Guid.NewGuid();
            appointments.Status = Appointments.AppointmentStatus.Pending;
            context.Appointments.Add(appointments);
            await context.SaveChangesAsync();
            return Results.Created($"/api/create/appointment", appointments);
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Patient"}).WithTags("Appointment").WithSummary("Creates a new appointment.").WithDescription("Creates a new appointment in the database with the provided information.");
        
        app.MapPost("/api/create/record", async (MedicalRecords medicalRecords, AppDbContext context) =>
        {
            medicalRecords.Id = Guid.NewGuid();
            context.MedicalRecords.Add(medicalRecords);
            await context.SaveChangesAsync();
            return Results.Created($"/api/create/record/{medicalRecords.Id}", medicalRecords);
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Patient"}).WithTags("MedicalRecord").WithSummary("Creates a new medical record.").WithDescription("Creates a new medical record in the database for the specified appointment based on its ID.");
        
        app.MapPut("/api/update/appointment/{id:guid}", async (Guid id, Appointments appointment, AppDbContext context) =>
        {
            var result = await context.Appointments.FindAsync(id);
            if (result == null)
            {
                return Results.NotFound($"Appointment with ID {id} not found.");
            }
            result.PatientId = appointment.PatientId;
            result.DoctorId = appointment.DoctorId;
            result.ScheduleTime = appointment.ScheduleTime;
            result.Status = appointment.Status;
            result.SymptomsDescription = appointment.SymptomsDescription;
            
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization().WithTags("Appointment").WithSummary("Updates an existing appointment.").WithDescription("Updates an existing appointment in the database with the provided information.");
        
        app.MapPut("/api/update/record/{id:guid}", async (Guid id, MedicalRecords medicalRecords, AppDbContext context) =>
        { 
            var result = await context.MedicalRecords.FindAsync(id);
            if (result == null)
            {
                return Results.NotFound($"Medical record with ID {id} not found.");
            }
            result.Id = medicalRecords.Id;
            result.AppointmentId = medicalRecords.AppointmentId;
            result.Filename = medicalRecords.Filename;
            result.Filepath = medicalRecords.Filepath;
            result.UploadedAt = medicalRecords.UploadedAt;
            
            await context.SaveChangesAsync();
            return Results.Ok(medicalRecords);
        }).RequireAuthorization().WithTags("MedicalRecord").WithSummary("Updates an existing medical record.").WithDescription("Updates an existing medical record in the database for the specified appointment based on its ID.");
        
        app.MapDelete("/api/delete/appointment/{id}", async (Guid id, AppDbContext context) =>
        {
            var appointment = await context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return Results.NotFound($"Cannot find appointment with id {id}");
            }
            context.Appointments.Remove(appointment);
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization().WithTags("Appointment").WithSummary("Deletes an existing appointment.").WithDescription("Deletes an existing appointment from the database based on its ID.");
        
        app.MapDelete("/api/delete/record/{id}", async (Guid id, AppDbContext context) =>
        {
            var records = await context.MedicalRecords.FindAsync(id);
            if (records == null)
            {
                return Results.NotFound($"Cannot find medical record with id {id}");
            }
            context.MedicalRecords.Remove(records);
            await context.SaveChangesAsync();
            return Results.Ok();
        }).RequireAuthorization(new AuthorizeAttribute {Roles ="Patient"}).WithTags("MedicalRecord").WithSummary("Deletes an existing medical record.").WithDescription("Deletes an existing medical record from the database for the specified appointment based on its ID.");
    }
}