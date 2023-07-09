using Service.Banking.Interfaces;
using Service.Common;

namespace Service.Banking.CkoBankSimulator;

public class CkoBankSimulator : IBankingThirdPartyService
{
    public Task<BankPaymentResult> ProcessPaymentAsync(Domain.Payment paymentRequest)
    {
        // simulate a long operation between 1 and 5 seconds
        var random = new Random();
        Thread.Sleep(random.Next(1,5) * 1000);

        // simulate a random outcome
        return random.Next(2) == 1
            ? Task.FromResult(new BankPaymentResult
                {
                    PaymentId = paymentRequest.Id,
                    Status = Status.Approved,
                    Reason = string.Empty
                })
            : Task.FromResult(new BankPaymentResult
                {
                    PaymentId = paymentRequest.Id,
                    Status = Status.Declined,
                    Reason = "Payment declined"
                });
    }
}