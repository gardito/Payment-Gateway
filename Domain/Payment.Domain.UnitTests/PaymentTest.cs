using Common;
using Domain.ValueObjects;

namespace Domain.UnitTests;

public class PaymentTest
{
    public class Create
    {
        public class OnValidInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var now = DateTime.Now;
                var paymentResult = PaymentHelper.MockPaymentResult(now);
                
                Assert.IsType<SuccessfulResult<Payment>>(paymentResult);
                Assert.Equal(now.ToUniversalTime(), paymentResult.Data.Date);
                Assert.Equal("Test Merchant", paymentResult.Data.Merchant.Name);
                Assert.Equal(10, paymentResult.Data.Transaction.Amount);
                Assert.Equal(Currency.GBP, paymentResult.Data.Transaction.Currency);
                Assert.Equal("4234123412341234", paymentResult.Data.Card.CardNumber.Get);
                Assert.Equal("John Doe", paymentResult.Data.Card.OwnerFullName.Get);
                Assert.Equal(Payment.PaymentStatus.Pending, paymentResult.Data.Status);
            }
        }
    }

    public class PaymentOperations
    {
        private static readonly Payment MockPayment = PaymentHelper.MockPaymentResult(DateTime.Now).Data;
        
        public class ApprovePayment
        {
            [Fact]
            public void SetsThePaymentToApproved()
            {
                MockPayment.ApprovePayment();
                Assert.Equal(Payment.PaymentStatus.Approved, MockPayment.Status);
            }
        }
        
        public class DeclinePayment
        {
            [Fact]
            public void SetsThePaymentToDeclined()
            {
                MockPayment.DeclinePayment();
                Assert.Equal(Payment.PaymentStatus.Declined, MockPayment.Status);
            }
        }
        
        public class ProcessPayment
        {
            [Fact]
            public void SetsThePaymentToInProgress()
            {
                MockPayment.ProcessPayment();
                Assert.Equal(Payment.PaymentStatus.InProgress, MockPayment.Status);
            }
        }
    }
}