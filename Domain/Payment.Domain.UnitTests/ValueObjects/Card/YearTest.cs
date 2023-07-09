using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class YearTest
{
    public class OnValidInputs
    {
        [Fact]
        public void ReturnsSuccessfulResult()
        {
            var yearResult = Year.Create("2023");
            Assert.IsType<SuccessfulResult<Year>>(yearResult);
            Assert.Equal(2023, yearResult.Data.Get);
        }
    }

    public class OnInvalidInputs
    {
        public class OnEmptyOrNullInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var yearResult1 = Year.Create("");
                AssertErrorResult(yearResult1);
                
                var yearResult2 = Year.Create("          ");
                AssertErrorResult(yearResult2);
                
                var yearResult3 = Year.Create(null);
                AssertErrorResult(yearResult3);
            }
        }
        
        public class OnInvalidLength
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var yearResult = Year.Create("202");
                AssertErrorResult(yearResult);
            }
        }
        
        public class OnNonDigitsInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var yearResult = Year.Create("2o23");
                AssertErrorResult(yearResult);
            }
        }
        
        private static void AssertErrorResult(Result<Year> yearResult)
        {
            Assert.IsType<ErrorResult<Year>>(yearResult);
            Assert.IsType<DateError>(yearResult.Error);
            Assert.Equal(ErrorCode.InvalidDate, yearResult.Error.ErrorCode);
            Assert.False(string.IsNullOrWhiteSpace(yearResult.Error.ErrorMessage));
            Assert.False(yearResult.IsSuccessful);
        }
    }
}