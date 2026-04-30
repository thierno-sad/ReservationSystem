using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationService.Data;
using ReservationService.Models;

namespace ReservationService.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly ReservationDbContext _context;

    public ReservationController(ReservationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations()
    {
        return Ok(await _context.Reservations.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(Reservation reservation)
    {
        reservation.Status = "Created";

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
    {
        var existingReservation = await _context.Reservations.FindAsync(id);

        if (existingReservation == null)
            return NotFound();

        existingReservation.ClientName = reservation.ClientName;
        existingReservation.ResourceName = reservation.ResourceName;
        existingReservation.Date = reservation.Date;
        existingReservation.Status = reservation.Status;

        await _context.SaveChangesAsync();

        return Ok(existingReservation);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null)
            return NotFound();

        reservation.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Réservation annulée avec succès",
            reservation
        });
    }
}