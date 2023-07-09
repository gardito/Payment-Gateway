using Common;
using Domain.ValueObjects;

namespace Data.Payment.InMemoryDb.IntegrationTest;

public class PaymentInMemoryDbTest
{
    private static Result<Domain.Payment> MockPaymentResult(DateTime now)
    {
        var mockCard = Card.Create("John Doe", "1234123412341234", "111", "12", "2030");
        return Domain.Payment.Create(now, new Domain.Merchant(1, "Test Merchant"), 10, Currency.GBP, mockCard.Data);
    }

    public class GetByIdAsync
    {
        public class OnExistingPayment
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            private Domain.Payment _existingPayment = MockPaymentResult(DateTime.Now).Data;
                
            public OnExistingPayment()
            {
                var _ = _inMemoryDb.SaveAsync(_existingPayment).Result;
            }
            
            [Fact]
            public async Task ReturnsSuccessfulResult()
            {
                var getByResult = await _inMemoryDb.GetByIdAsync(_existingPayment.Id);
                
                Assert.IsType<SuccessfulResult<Domain.Payment>>(getByResult);
                Assert.True(getByResult.IsSuccessful);
                Assert.Equal(_existingPayment.Id, getByResult.Data.Id);
            }
        }

        public class OnMissingPayment
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            
            [Fact]
            public async Task ReturnsErrorResult()
            {
                var paymentToGet = MockPaymentResult(DateTime.Now).Data;
                var getByResult = await _inMemoryDb.GetByIdAsync(paymentToGet.Id);
                
                Assert.IsType<ErrorResult<Domain.Payment>>(getByResult);
                Assert.False(getByResult.IsSuccessful);
            }
        }
    }
    
    public class UpdateAsync
    {
        public class OnFirstSave
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            
            [Fact]
            public async Task ReturnsErrorResult()
            {
                var paymentToSave = MockPaymentResult(DateTime.Now).Data;
                var updateResult = await _inMemoryDb.UpdateAsync(paymentToSave);
                
                Assert.IsType<ErrorResult<Domain.Payment>>(updateResult);
                Assert.False(updateResult.IsSuccessful);
            }
        }
        
        public class OnExistingPayment
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            private Domain.Payment _existingPayment = MockPaymentResult(DateTime.Now).Data;
                
            public OnExistingPayment()
            {
                var _ = _inMemoryDb.SaveAsync(_existingPayment).Result;
                _existingPayment.ApprovePayment();
            }
            
            [Fact]
            public async Task ReturnsSuccessfulResult()
            {
                var updateResult = await _inMemoryDb.UpdateAsync(_existingPayment);
                
                Assert.IsType<SuccessfulResult<Domain.Payment>>(updateResult);
                Assert.True(updateResult.IsSuccessful);
                Assert.Equal(_existingPayment.Id, updateResult.Data.Id);
                Assert.Equal(Domain.Payment.PaymentStatus.Approved, updateResult.Data.Status);
            }
        }
    }
    
    public class SaveAsync
    {
        public class OnFirstSave
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            
            [Fact]
            public async Task ReturnsSuccessfulResult()
            {
                var paymentToSave = MockPaymentResult(DateTime.Now).Data;
                var saveResult = await _inMemoryDb.SaveAsync(paymentToSave);
                
                Assert.IsType<SuccessfulResult<Domain.Payment>>(saveResult);
                Assert.True(saveResult.IsSuccessful);
                Assert.Equal(paymentToSave.Id, saveResult.Data.Id);
            }
        }
        
        public class OnExistingPayment
        {
            private PaymentInMemoryDb _inMemoryDb = new PaymentInMemoryDb();
            private Domain.Payment _existingPayment = MockPaymentResult(DateTime.Now).Data;
                
            public OnExistingPayment()
            {
                var _ = _inMemoryDb.SaveAsync(_existingPayment).Result;
            }
            
            [Fact]
            public async Task ReturnsErrorResult()
            {
                var saveResult = await _inMemoryDb.SaveAsync(_existingPayment);
                
                Assert.IsType<ErrorResult<Domain.Payment>>(saveResult);
                Assert.False(saveResult.IsSuccessful);
            }
        }
    }
}