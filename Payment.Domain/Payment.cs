using Common;
using Domain.ValueObjects;

namespace Domain;

public class Payment
{
    public readonly Guid Id;
    public readonly DateTime Date;
    public readonly Merchant Merchant;
    public readonly Transaction Transaction;
    public readonly Card Card;
    public PaymentStatus Status;

    public enum PaymentStatus
    {
        Pending,
        InProgress,
        Approved,
        Declined
    }

    private Payment(DateTime dateTime, Merchant merchant, Transaction transaction, Card card)
    {
        Id = Guid.NewGuid();
        Date = dateTime.ToUniversalTime();
        Merchant = merchant;
        Transaction = transaction;
        Card = card;
        Status = PaymentStatus.Pending;
    }

    public static Result<Payment> Create(DateTime dateTime, Merchant merchant, decimal amount, Currency currency, Card card)
    {
        var transactionResult = Transaction.Create(amount, currency);
        
        if (!transactionResult.IsSuccessful)
            return new ErrorResult<Payment>(transactionResult.Error);

        return new SuccessfulResult<Payment>(
            new Payment(
                dateTime,
                merchant,
                transactionResult.Data,
                card));
    }

    public void ApprovePayment()
    {
        Status = PaymentStatus.Approved;
    }

    public void DeclinePayment()
    {
        Status = PaymentStatus.Declined;
    }
    
    public void ProcessPayment()
    {
        Status = PaymentStatus.InProgress;
    }
    
    public void ResetStatus()
    {
        Status = PaymentStatus.Pending;
    }

    public bool IsInProgress => Status == PaymentStatus.InProgress;
    public bool IsSettled => Status is PaymentStatus.Approved or PaymentStatus.Declined;
}