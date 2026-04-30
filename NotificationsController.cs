using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _context;

    public NotificationsController(NotificationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        return Ok(await _context.Notifications.ToListAsync());
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification(NotificationRecord request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) &&
            string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return BadRequest("Veuillez fournir un email ou un numéro de téléphone.");
        }

        request.Status = "Sent";
        request.SentAt = DateTime.Now;

        _context.Notifications.Add(request);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            notificationId = request.Id,
            email = request.Email,
            phoneNumber = request.PhoneNumber,
            message = request.Message,
            status = request.Status,
            sentAt = request.SentAt,
            detail = "Notification fictive envoyée et enregistrée avec succès."
        });
    }
}