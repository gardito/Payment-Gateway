using Common;
using Database.Common.Errors;
using Domain.Repositories;
using Domain.Services;
using Service.Banking.CkoBankSimulator.Errors;
using Service.Banking.Interfaces;
using Service.Common;

namespace Payment.Service;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBankingThirdPartyService _bankingThirdPartyService;

    public PaymentService(IPaymentRepository paymentRepository, IBankingThirdPartyService bankingThirdPartyService)
    {
        _paymentRepository = paymentRepository;
        _bankingThirdPartyService = bankingThirdPartyService;
    }
    
    public async Task<Result<Domain.Payment>> GetPaymentAsync(Guid id)
    {
        try
        {
            var getPaymentResult = await _paymentRepository.GetByIdAsync(id);

            if (getPaymentResult.IsSuccessful)
                return new SuccessfulResult<Domain.Payment>(getPaymentResult.Data);

            return new ErrorResult<Domain.Payment>(getPaymentResult.Error);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ErrorResult<Domain.Payment>(new DatabaseError(e.Message));
        }
    }

    public async Task<Result<BankPaymentResult>> ProcessPaymentAsync(Domain.Payment payment)
    {
        // the service only accepts payments in their initial state (= Pending)
        if(payment.Status != Domain.Payment.PaymentStatus.Pending)
            return new ErrorResult<BankPaymentResult>(new PaymentError("Invalid status for payment"));
            
        var getPaymentResult = await _paymentRepository.GetByIdAsync(payment.Id);

        // a record exists and in progress so no need to process the payment
        if (getPaymentResult.IsSuccessful && getPaymentResult.Data.IsInProgress)
            return new ErrorResult<BankPaymentResult>(new PaymentError("Payment is in progress"));
            
        // the payment is processed already
        if(getPaymentResult.IsSuccessful && getPaymentResult.Data.IsSettled)
            return new ErrorResult<BankPaymentResult>(new PaymentError("Payment is settled"));
            
        try
        {
            // creating the payment record for the first time
            await _paymentRepository.SaveAsync(payment);
                
            payment.ProcessPayment();
            await _paymentRepository.UpdateAsync(payment);
                
            var processPaymentResult = await _bankingThirdPartyService.ProcessPaymentAsync(payment);

            if (processPaymentResult.IsApproved())
            {
                payment.ApprovePayment();
                await _paymentRepository.UpdateAsync(payment);
                return new SuccessfulResult<BankPaymentResult>(processPaymentResult);
            }

            payment.DeclinePayment();
            await _paymentRepository.UpdateAsync(payment);

            return new ErrorResult<BankPaymentResult>(new BankError(processPaymentResult.Reason));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // an exception from the banking provider or from the database happened.
            // reset the status to "pending" for later processing
            payment.ResetStatus();
            // this can throw an exception too
            await _paymentRepository.UpdateAsync(payment);
            return new ErrorResult<BankPaymentResult>(new SystemError(e.Message));
        }

        return new ErrorResult<BankPaymentResult>(getPaymentResult.Error);
    }
}