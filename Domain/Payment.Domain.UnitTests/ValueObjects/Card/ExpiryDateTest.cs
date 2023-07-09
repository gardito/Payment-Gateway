using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class ExpiryDateTest
{
    public class Create
    {
        public class OnValidInputs
        {
            public class OnFutureDate
            {
                [Fact]
                public void ReturnsSuccessfulResult()
                {
                    var month = Month.Create("July").Data;
                    var year = Year.Create("2030").Data;
                    var expiryDateResult = ExpiryDate.Create(month, year);

                    Assert.IsType<SuccessfulResult<ExpiryDate>>(expiryDateResult);
                    Assert.Equal(7, expiryDateResult.Data.Month.Get);
                    Assert.IsType<Month>(expiryDateResult.Data.Month);
                    Assert.Equal(2030, expiryDateResult.Data.Year.Get);
                    Assert.IsType<Year>(expiryDateResult.Data.Year);
                }
            }
            
            public class OnSameMonthThisYear
            {
                [Fact]
                public void ReturnsSuccessfulResult()
                {
                    var thisMonth = DateTime.Today.Month.ToString();
                    var thisYear = DateTime.Today.Year.ToString();
                    var month = Month.Create(thisMonth).Data;
                    var year = Year.Create(thisYear).Data;
                    var expiryDateResult = ExpiryDate.Create(month, year);

                    Assert.IsType<SuccessfulResult<ExpiryDate>>(expiryDateResult);
                    Assert.Equal(int.Parse(thisMonth), expiryDateResult.Data.Month.Get);
                    Assert.IsType<Month>(expiryDateResult.Data.Month);
                    Assert.Equal(int.Parse(thisYear), expiryDateResult.Data.Year.Get);
                    Assert.IsType<Year>(expiryDateResult.Data.Year);
                }
            }
        }

        public class OnInvalidInputs
        {
            public class OnPastMonthThisYear
            {
                [Fact]
                public void ReturnsErrorResult()
                {
                    var lastMonth = DateTime.Today.AddMonths(-1).Month.ToString();
                    var thisYear = DateTime.Today.Year.ToString();
                    var month = Month.Create(lastMonth).Data;
                    var year = Year.Create(thisYear).Data;
                    var expiryDateResult = ExpiryDate.Create(month, year);

                    AssertErrorResult(expiryDateResult);
                }
            }

            public class OnPastYear
            {
                [Fact]
                public void ReturnsErrorResult()
                {
                    var lastMonth = DateTime.Today.Month.ToString();
                    var thisYear = DateTime.Today.AddYears(-2).Year.ToString();
                    var month = Month.Create(lastMonth).Data;
                    var year = Year.Create(thisYear).Data;
                    var expiryDateResult = ExpiryDate.Create(month, year);

                    AssertErrorResult(expiryDateResult);
                }
            }

            private static void AssertErrorResult(Result<ExpiryDate> expiryDateResult)
            {
                Assert.IsType<ErrorResult<ExpiryDate>>(expiryDateResult);
                Assert.IsType<ExpiryDateError>(expiryDateResult.Error);
                Assert.Equal(ErrorCode.InvalidExpiryDate, expiryDateResult.Error.ErrorCode);
                Assert.False(string.IsNullOrWhiteSpace(expiryDateResult.Error.ErrorMessage));
                Assert.False(expiryDateResult.IsSuccessful);
            }
        }
    }
}