using Common;
using Domain.ValueObjects;

namespace Domain.UnitTests;

public class PaymentHelper
{
    public static Result<Payment> MockPaymentResult(DateTime now)
    {
        var mockCard = Card.Create("John Doe", "4234123412341234", "111", "12", "2030");
        return Payment.Create(now, new Merchant(1, "Test Merchant"), 10, Currency.GBP, mockCard.Data);
    }
}