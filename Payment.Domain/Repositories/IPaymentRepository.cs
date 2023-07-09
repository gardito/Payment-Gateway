using Common;

namespace Domain.Repositories;

public interface IPaymentRepository
{
    public Task<Result<Payment>> GetByIdAsync(Guid id);
    public Task<Result<Payment>> UpdateAsync(Payment payment);
    public Task<Result<Payment>> SaveAsync(Payment payment);
}