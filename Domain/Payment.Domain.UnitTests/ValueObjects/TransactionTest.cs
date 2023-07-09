using Common;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class TransactionTest
{
    public class Create
    {
        public class OnValidInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var transaction = Transaction.Create(100, Currency.GBP);
                Assert.IsType<SuccessfulResult<Transaction>>(transaction);
                Assert.Equal(100, transaction.Data.Amount);
                Assert.Equal(Currency.GBP, transaction.Data.Currency);
            }
        }
    }
}