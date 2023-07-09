using System.Globalization;
using Cache.Common;
using Common.Errors;
using Domain;
using Domain.Services;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.WebApi.DTOs;

namespace PaymentGateway.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IIdempotencyCachingSystem _idempotencyCachingSystem;

    public PaymentController(IPaymentService paymentService, IIdempotencyCachingSystem idempotencyCachingSystem)
    {
        _paymentService = paymentService;
        _idempotencyCachingSystem = idempotencyCachingSystem;
    }
    
    // get payment
    [HttpGet("{guid}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentById(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
            return BadRequest("Empty id");
        
        if (!Guid.TryParse(guid, out var parsedGuid))
            return BadRequest("Invalid id format");

        try
        {
            var paymentResult = await _paymentService.GetPaymentAsync(parsedGuid);

            if (!paymentResult.IsSuccessful && paymentResult.Error.ErrorCode == ErrorCode.PaymentNotFound)
                return NotFound(paymentResult.Error.ErrorMessage);

            var payment = paymentResult.Data;
            var paymentDto = new PaymentDto
            {
                Id = payment.Id.ToString(),
                TransactionDate = payment.Date.ToString(CultureInfo.InvariantCulture),
                MerchantId = payment.Merchant.Id,
                MerchantName = payment.Merchant.Name,
                Amount = payment.Transaction.Amount,
                Currency = payment.Transaction.Currency.ToString(),
                CardNumber = payment.Card.GetMaskedNumber(),
                Status = payment.Status.ToString()
            };
                
            return Ok(paymentDto);
        }
        catch (Exception e)
        {   
            Console.WriteLine(e);
        }

        return BadRequest("Unknown error");
    }

    [HttpPost]
    public async Task<ActionResult> ProcessPayment([FromBody] InputPaymentDto input)
    {
        if (await _idempotencyCachingSystem.Contains(input.IdempotencyId))
            return BadRequest("Payment processed already");
        
        var cardDto = input.Card;
        var createCardResult = Card.Create(
            cardDto.OwnerFullName,
            cardDto.CardNumber.Replace(" ", string.Empty),
            cardDto.Cvv,
            cardDto.ExpiryMonth,
            cardDto.ExpiryYear);

        if (!createCardResult.IsSuccessful)
            return BadRequest(createCardResult.Error.ErrorMessage);

        if (!IsValidCurrency(input.Currency, out var currency))
            return BadRequest("Invalid payment currency");

        var now = DateTime.Now.ToUniversalTime();
        var createPaymentResult = Domain.Payment.Create(
            now, 
            new Merchant(input.MerchantId,input.MerchantName), 
            input.Amount,
            currency,
            createCardResult.Data
            );

        if (!createPaymentResult.IsSuccessful)
            return BadRequest(createPaymentResult.Error.ErrorMessage);
        
        var processPaymentResult = await _paymentService.ProcessPaymentAsync(createPaymentResult.Data);

        await _idempotencyCachingSystem.Add(input.IdempotencyId);

        if (!processPaymentResult.IsSuccessful)
            return BadRequest(
                new { 
                    PaymentId = createPaymentResult.Data.Id,
                    ErrorMessage = processPaymentResult.Error.ErrorMessage 
                });

        return Ok(new { PaymentId = processPaymentResult.Data.PaymentId });
    }

    private static bool IsValidCurrency(string currency, out Currency parsedCurrency)
    {
        return Enum.TryParse(currency, true, out parsedCurrency);
    }
}