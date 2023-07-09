namespace Service.Common;

public enum Status
{
    Approved,
    Declined
}

public class BankPaymentResult
{
    public Guid PaymentId { get; set; }
    public Status Status { get; set; }
    public string Reason { get; set; }

    public bool IsApproved() => Status == Status.Approved;
}