using Common;
using Domain.ValueObjects;

namespace Payment.Domain.UnitTests.ValueObjects.Card;

public class CardProviderTest
{
    public class GetProvider
    {
        public class OnNumberStartingWith2Or5
        {
            [Fact]
            public void ReturnsMastercard()
            {
                var masterCardWith2 = CardNumber.Create("2134123412341234").Data;
                var result = CardProvider.GetProvider(masterCardWith2);
                Assert.IsType<SuccessfulResult<CardProviderEnum>>(result);
                Assert.Equal(CardProviderEnum.Mastercard, result.Data);

                var masterCardWith5 = CardNumber.Create("5134123412341234").Data;
                var result2 = CardProvider.GetProvider(masterCardWith5);
                Assert.IsType<SuccessfulResult<CardProviderEnum>>(result2);
                Assert.Equal(CardProviderEnum.Mastercard, result.Data);
            }
        }
        
        public class OnNumberStartingWith4
        {
            [Fact]
            public void ReturnsVisa()
            {
                var visa = CardNumber.Create("4134123412341234").Data;
                var result = CardProvider.GetProvider(visa);
                Assert.IsType<SuccessfulResult<CardProviderEnum>>(result);
                Assert.Equal(CardProviderEnum.Visa, result.Data);
            }
        }
        
        public class OnNumberStartingWith34Or37
        {
            [Fact]
            public void ReturnsAmex()
            {
                var amexWith34 = CardNumber.Create("3434123412341234").Data;
                var result = CardProvider.GetProvider(amexWith34);
                Assert.IsType<SuccessfulResult<CardProviderEnum>>(result);
                Assert.Equal(CardProviderEnum.AmericanExpress, result.Data);

                var amexWith37 = CardNumber.Create("3734123412341234").Data;
                var result2 = CardProvider.GetProvider(amexWith37);
                Assert.IsType<SuccessfulResult<CardProviderEnum>>(result2);
                Assert.Equal(CardProviderEnum.AmericanExpress, result.Data);
            }
        }

    }
}