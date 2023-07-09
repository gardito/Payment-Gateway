namespace PaymentGateway.WebApi.DTOs;

public class InputPaymentDto
{
    public Guid IdempotencyId { get; set; }
    public int MerchantId { get; set; }
    public string MerchantName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public InputCardDto Card { get; set; }
}

public class InputCardDto
{
    public string OwnerFullName { get; set; }
    public string CardNumber { get; set; }
    public string Cvv { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
}