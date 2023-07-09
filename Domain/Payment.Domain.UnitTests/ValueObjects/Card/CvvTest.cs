using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class CvvTest
{
    public class Create
    {
        public class OnValidInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var visaOrMastercardCvv = Cvv.Create("123");
                Assert.IsType<SuccessfulResult<Cvv>>(visaOrMastercardCvv);
                Assert.Equal(123, visaOrMastercardCvv.Data.Get);

                var amexCvv = Cvv.Create("1234");
                Assert.IsType<SuccessfulResult<Cvv>>(amexCvv);
                Assert.Equal(1234, amexCvv.Data.Get);
            }
        }
        
        public class OnEmptyOrNullInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var cvv1 = Cvv.Create("");
                AssertErrorResult(cvv1);
                
                var cvv2 = Cvv.Create("          ");
                AssertErrorResult(cvv2);
                
                var cvv3 = Cvv.Create(null);
                AssertErrorResult(cvv3);
            }
        }
        
        public class OnNonDigitsInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var cvvResult = Cvv.Create("1w3");
                AssertErrorResult(cvvResult);
            }
        }

        public class OnInvalidLength
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                const string tenDigitsNumber = "1234567890"; 
                var cvvResult = Cvv.Create(tenDigitsNumber);
                AssertErrorResult(cvvResult);
            }
        }

        private static void AssertErrorResult(Result<Cvv> cvvResult)
        {
            Assert.IsType<ErrorResult<Cvv>>(cvvResult);
            Assert.IsType<CvvError>(cvvResult.Error);
            Assert.Equal(ErrorCode.InvalidCvv, cvvResult.Error.ErrorCode);
            Assert.False(string.IsNullOrWhiteSpace(cvvResult.Error.ErrorMessage));
            Assert.False(cvvResult.IsSuccessful);
        }
    }
}