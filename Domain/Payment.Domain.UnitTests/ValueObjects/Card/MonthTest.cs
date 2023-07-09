using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class MonthTest
{
    public class OnValidInputs
    {
        [Fact]
        public void ReturnsSuccessfulResult()
        {
            var monthStringResult = Month.Create("July");
            Assert.IsType<SuccessfulResult<Month>>(monthStringResult);
            Assert.Equal(7, monthStringResult.Data.Get);

            var monthStringInsResult = Month.Create("july");
            Assert.IsType<SuccessfulResult<Month>>(monthStringInsResult);
            Assert.Equal(7, monthStringInsResult.Data.Get);

            var monthIntResult = Month.Create("7");
            Assert.IsType<SuccessfulResult<Month>>(monthIntResult);
            Assert.Equal(7, monthIntResult.Data.Get);
        }
    }

    public class OnInvalidInputs
    {
        public class OnEmptyOrNullInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var monthResult1 = Month.Create("");
                AssertErrorResult(monthResult1);
                
                var monthResult2 = Month.Create("          ");
                AssertErrorResult(monthResult2);
                
                var monthResult3 = Month.Create(null);
                AssertErrorResult(monthResult3);
            }
        }
        
        public class OnInvalidInteger
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var monthResult = Month.Create("13");
                AssertErrorResult(monthResult);
            }
        }
        
        public class OnInvalidString
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var monthResult = Month.Create("Not a month");
                AssertErrorResult(monthResult);
            }
        }
        
        private static void AssertErrorResult(Result<Month> monthResult)
        {
            Assert.IsType<ErrorResult<Month>>(monthResult);
            Assert.IsType<DateError>(monthResult.Error);
            Assert.Equal(ErrorCode.InvalidDate, monthResult.Error.ErrorCode);
            Assert.False(string.IsNullOrWhiteSpace(monthResult.Error.ErrorMessage));
            Assert.False(monthResult.IsSuccessful);
        }
    }
}