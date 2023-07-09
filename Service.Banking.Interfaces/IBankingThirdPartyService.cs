using Domain;
using Service.Common;

namespace Service.Banking.Interfaces;

public interface IBankingThirdPartyService
{
    public Task<BankPaymentResult> ProcessPaymentAsync(Payment paymentRequest);
}