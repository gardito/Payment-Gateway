using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class CardNumberTest
{
    public class Create
    {
        public class OnVisaOrMastercardInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                const string sixteenDigitsNumber = "1234123412341234";
                var cardNumberResult = CardNumber.Create(sixteenDigitsNumber);
                Assert.IsType<SuccessfulResult<CardNumber>>(cardNumberResult);
                Assert.Equal(sixteenDigitsNumber, cardNumberResult.Data.Get);
            }
        }
        
        public class OnAmexInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                const string fifteenDigitsNumber = "123412341234123";
                var cardNumberResult = CardNumber.Create(fifteenDigitsNumber);
                Assert.IsType<SuccessfulResult<CardNumber>>(cardNumberResult);
                Assert.Equal(fifteenDigitsNumber, cardNumberResult.Data.Get);
            }
        }

        public class OnEmptyOrNullInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var cardNumberResult1 = CardNumber.Create("");
                AssertErrorResult(cardNumberResult1);
            
                var cardNumberResult2 = CardNumber.Create("          ");
                AssertErrorResult(cardNumberResult2);
            
                var cardNumberResult3 = CardNumber.Create(null);
                AssertErrorResult(cardNumberResult3);
            }
        }
        
        public class OnNonDigitsInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var cardNumberResult = CardNumber.Create("1234test12341234");
                AssertErrorResult(cardNumberResult);
            }
        }
        
        public class OnInvalidLength
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                const string tenDigitsNumber = "1234567890"; 
                var cardNumberResult = CardNumber.Create(tenDigitsNumber);
                AssertErrorResult(cardNumberResult);
            }
        }
        
        private static void AssertErrorResult(Result<CardNumber> cardNumberResult)
        {
            Assert.IsType<ErrorResult<CardNumber>>(cardNumberResult);
            Assert.IsType<CardNumberError>(cardNumberResult.Error);
            Assert.Equal(ErrorCode.InvalidCardNumber, cardNumberResult.Error.ErrorCode);
            Assert.False(string.IsNullOrWhiteSpace(cardNumberResult.Error.ErrorMessage));
            Assert.False(cardNumberResult.IsSuccessful);
        }
    }
}
