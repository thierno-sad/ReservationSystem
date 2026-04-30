using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Models;
using Stripe;
using Microsoft.EntityFrameworkCore;
using PaymentRecord = PaymentService.Models.PaymentRecord;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly PaymentDbContext _context;

    public PaymentController(IConfiguration configuration, PaymentDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        return Ok(await _context.Payments.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Pay()
    {
        var secretKey = _configuration["Stripe:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
            return BadRequest("Clé Stripe manquante.");

        StripeConfiguration.ApiKey = secretKey;

        var options = new PaymentIntentCreateOptions
        {
            Amount = 1000,
            Currency = "cad",
            PaymentMethod = "pm_card_visa",
            Confirm = true,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            }
        };

        var service = new PaymentIntentService();
        var intent = service.Create(options);

        var paymentRecord = new PaymentRecord
        {
            StripePaymentId = intent.Id,
            Amount = intent.Amount,
            Currency = intent.Currency,
            Status = intent.Status,
            CreatedAt = DateTime.Now
        };

        _context.Payments.Add(paymentRecord);
        await _context.SaveChangesAsync();

        if (intent.Status == "succeeded")
        {
            return Ok(new
            {
                paymentRecord.Id,
                stripeId = intent.Id,
                intent.Amount,
                intent.Currency,
                intent.Status,
                message = "Paiement validé"
            });
        }

        return BadRequest(new
        {
            paymentRecord.Id,
            stripeId = intent.Id,
            intent.Status,
            message = "Paiement non validé"
        });
    }
}