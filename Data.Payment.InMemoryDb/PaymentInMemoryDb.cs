using Common;
using Database.Common.Errors;
using Domain.Repositories;

namespace Data.Payment.InMemoryDb;

public class PaymentInMemoryDb : IPaymentRepository
{
    private Dictionary<Guid, Domain.Payment> _database;

    public PaymentInMemoryDb()
    {
        // key: payment id / value: payment
        _database = new Dictionary<Guid, Domain.Payment>();
    }
    
    public Task<Result<Domain.Payment>> GetByIdAsync(Guid id)
    {
        try
        {
            if (!_database.ContainsKey(id))
                return Task.FromResult<Result<Domain.Payment>>(
                    new ErrorResult<Domain.Payment>(new DatabaseError($"Payment with {id} not found")));
            
            return Task.FromResult<Result<Domain.Payment>>(new SuccessfulResult<Domain.Payment>(_database[id]));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult<Result<Domain.Payment>>(
                new ErrorResult<Domain.Payment>(new DatabaseError("An error occured")));
        }
    }

    public Task<Result<Domain.Payment>> UpdateAsync(Domain.Payment payment)
    {
        try
        {
            if (_database.ContainsKey(payment.Id))
            {
                _database[payment.Id] = payment;
                return Task.FromResult<Result<Domain.Payment>>(new SuccessfulResult<Domain.Payment>(payment));
            }
                
            return Task.FromResult<Result<Domain.Payment>>(
                new ErrorResult<Domain.Payment>(new DatabaseError($"Payment with {payment.Id} doesn't exists")));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult<Result<Domain.Payment>>(
                new ErrorResult<Domain.Payment>(new DatabaseError("An error occured")));
        }
    }

    public Task<Result<Domain.Payment>> SaveAsync(Domain.Payment payment)
    {
        try
        {
            if (!_database.ContainsKey(payment.Id))
            {
                _database.Add(payment.Id, payment);
                return Task.FromResult<Result<Domain.Payment>>(new SuccessfulResult<Domain.Payment>(payment));
            }
                
            return Task.FromResult<Result<Domain.Payment>>(
                new ErrorResult<Domain.Payment>(new DatabaseError($"Payment with {payment.Id} already exists")));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult<Result<Domain.Payment>>(
                new ErrorResult<Domain.Payment>(new DatabaseError("An error occured")));
        }
    }
}