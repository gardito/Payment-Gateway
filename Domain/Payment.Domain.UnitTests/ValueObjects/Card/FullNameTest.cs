using Common;
using Common.Errors;
using Domain.Common.Errors;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects.Card;

public class FullNameTest
{
    public class Create
    {
        public class OnValidInputs
        {
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var fullName = FullName.Create("John Doe");
                Assert.IsType<SuccessfulResult<FullName>>(fullName);
                Assert.Equal("John Doe", fullName.Data.Get);
            }
        }

        public class OnEmptyOrNullInput
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var fullName1 = FullName.Create("");
                AssertErrorResult(fullName1);
                
                var fullName2 = FullName.Create("          ");
                AssertErrorResult(fullName2);
                
                var fullName3 = FullName.Create(null);
                AssertErrorResult(fullName3);
            }
        }
        
        public class OnSingleNameProvided
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var fullName = FullName.Create("John");
                AssertErrorResult(fullName);
            }
        }
        
        public class OnInvalidCharactersProvided
        {
            [Fact]
            public void ReturnsErrorResult()
            {
                var fullName = FullName.Create("John Doe4@");
                AssertErrorResult(fullName);
            }
        }
        
        private static void AssertErrorResult(Result<FullName> fullnameResult)
        {
            Assert.IsType<ErrorResult<FullName>>(fullnameResult);
            Assert.IsType<FullNameError>(fullnameResult.Error);
            Assert.Equal(ErrorCode.InvalidName, fullnameResult.Error.ErrorCode);
            Assert.False(string.IsNullOrWhiteSpace(fullnameResult.Error.ErrorMessage));
            Assert.False(fullnameResult.IsSuccessful);
        }
    }
}