namespace PaymentGateway.WebApi.DTOs;

public class PaymentDto
{
    public string Id { get; set; }
    public string TransactionDate { get; set; }
    public int MerchantId { get; set; }
    public string MerchantName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CardNumber { get; set; }
    public string Status { get; set; }
}