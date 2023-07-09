using Common;
using Service.Common;

namespace Domain.Services;

public interface IPaymentService
{
    public Task<Result<Payment>> GetPaymentAsync(Guid id);
    public Task<Result<BankPaymentResult>> ProcessPaymentAsync(Payment payment);
}