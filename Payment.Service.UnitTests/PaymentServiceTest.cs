using Common;
using Database.Common.Errors;
using Domain.Repositories;
using Domain.ValueObjects;
using Moq;
using Service.Banking.Interfaces;
using Service.Common;

namespace Payment.Service.UnitTests;

public class PaymentServiceTest
{
    private static Result<Domain.Payment> MockPaymentResult(DateTime now)
    {
        var mockCard = Card.Create("John Doe", "1234123412341234", "111", "12", "2030");
        return Domain.Payment.Create(now, new Domain.Merchant(1, "Test Merchant"), 10, Currency.GBP, mockCard.Data);
    }

    public class GetPaymentAsync
    {
        private static Result<Domain.Payment> paymentResult;
        
        public class OnExistingPayment
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository;
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService;

            public OnExistingPayment()
            {
                paymentResult = MockPaymentResult(DateTime.Now);

                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(paymentResult);

                _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }

            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var result = _paymentService.GetPaymentAsync(It.IsAny<Guid>()).Result;
                Assert.IsType<SuccessfulResult<Domain.Payment>>(result);
                Assert.True(result.IsSuccessful);
            }
        }

        public class OnMissingPayment
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository;
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService;

            public OnMissingPayment()
            {
                var errorResult = new ErrorResult<Domain.Payment>(new DatabaseError("Not Found"));

                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(errorResult);

                _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }

            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.GetPaymentAsync(It.IsAny<Guid>()).Result;
                Assert.IsType<ErrorResult<Domain.Payment>>(result);
                Assert.IsType<DatabaseError>(result.Error);
                Assert.Equal("Not Found", result.Error.ErrorMessage);
                Assert.False(result.IsSuccessful);
            }
        }
        
        public class OnException
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository;
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService;

            public OnException()
            {
                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Throws(new Exception("Db fake error"));

                _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }

            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.GetPaymentAsync(It.IsAny<Guid>()).Result;
                Assert.IsType<ErrorResult<Domain.Payment>>(result);
                Assert.IsType<DatabaseError>(result.Error);
                Assert.Equal("Db fake error", result.Error.ErrorMessage);
                Assert.False(result.IsSuccessful);
            }
        }
    }
    
    public class ProcessPaymentAsync
    {
        public class OnInvalidPaymentState
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository = new Mock<IPaymentRepository>();
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();
            private Result<BankPaymentResult> bankPaymentResult;

            private Domain.Payment _inputPayment = MockPaymentResult(DateTime.Now).Data;

            public OnInvalidPaymentState()
            {
                _inputPayment.ApprovePayment();
                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }
            
            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.ProcessPaymentAsync(_inputPayment).Result;
                Assert.IsType<ErrorResult<BankPaymentResult>>(result);
                Assert.IsType<PaymentError>(result.Error);
            }
        }
        
        public class OnInProgressOrSettledPayment
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository = new Mock<IPaymentRepository>();
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

            private Domain.Payment _inputPayment = MockPaymentResult(DateTime.Now).Data;

            public OnInProgressOrSettledPayment()
            {
                var storedPayment = MockPaymentResult(DateTime.Now).Data;
                storedPayment.ApprovePayment();
                var inProgressPayment = new SuccessfulResult<Domain.Payment>(storedPayment);
                    
                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(inProgressPayment);
                
                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }
            
            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.ProcessPaymentAsync(_inputPayment).Result;
                Assert.IsType<ErrorResult<BankPaymentResult>>(result);
                Assert.IsType<PaymentError>(result.Error);
                
                _paymentRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            }
        }
        
        public class OnSuccessfulPayment
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository = new Mock<IPaymentRepository>();
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

            private Domain.Payment _inputPayment = MockPaymentResult(DateTime.Now).Data;
            private Result<Domain.Payment> storedPayment = MockPaymentResult(DateTime.Now);

            public OnSuccessfulPayment()
            {
                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(storedPayment);

                _bankingThirdPartyService
                    .Setup(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()))
                    .Returns(Task.FromResult(new BankPaymentResult()
                    {
                        PaymentId = _inputPayment.Id,
                        Status = Status.Approved,
                        Reason = ""
                    }));
                
                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }
            
            [Fact]
            public void ReturnsSuccessfulResult()
            {
                var result = _paymentService.ProcessPaymentAsync(_inputPayment).Result;
                Assert.IsType<SuccessfulResult<BankPaymentResult>>(result);
                Assert.True(result.IsSuccessful);
                
                _paymentRepository.Verify(repository => repository.SaveAsync(_inputPayment), Times.Once);
                _paymentRepository.Verify(repository => repository.UpdateAsync(_inputPayment), Times.Exactly(2));
                _bankingThirdPartyService.Verify(service => service.ProcessPaymentAsync(_inputPayment), Times.Once);
            }
        }

        public class OnRejectedPayment
        {
            private PaymentService _paymentService;
            private Mock<IPaymentRepository> _paymentRepository = new Mock<IPaymentRepository>();
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();

            private Domain.Payment _inputPayment = MockPaymentResult(DateTime.Now).Data;
            private Result<Domain.Payment> storedPayment = MockPaymentResult(DateTime.Now);
            
            public OnRejectedPayment()
            {
                _paymentRepository = new Mock<IPaymentRepository>();
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(storedPayment);

                _bankingThirdPartyService
                    .Setup(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()))
                    .Returns(Task.FromResult(new BankPaymentResult()
                    {
                        PaymentId = _inputPayment.Id,
                        Status = Status.Declined,
                        Reason = "Not enough balance"
                    }));
                
                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }

            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.ProcessPaymentAsync(_inputPayment).Result;
                Assert.IsType<ErrorResult<BankPaymentResult>>(result);
                Assert.False(result.IsSuccessful);
                
                _paymentRepository.Verify(repository => repository.SaveAsync(_inputPayment), Times.Once);
                _paymentRepository.Verify(repository => repository.UpdateAsync(_inputPayment), Times.Exactly(2));
                _bankingThirdPartyService.Verify(service => service.ProcessPaymentAsync(_inputPayment), Times.Once);
            }
        }
        
        public class OnException
        {
            private Mock<IPaymentRepository> _paymentRepository = new Mock<IPaymentRepository>();
            private Mock<IBankingThirdPartyService> _bankingThirdPartyService = new Mock<IBankingThirdPartyService>();
            private PaymentService _paymentService;

            private Domain.Payment _inputPayment = MockPaymentResult(DateTime.Now).Data;
            private Result<Domain.Payment> storedPayment = MockPaymentResult(DateTime.Now);

            public OnException()
            {
                _paymentRepository
                    .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()).Result)
                    .Returns(storedPayment);

                _bankingThirdPartyService
                    .Setup(service => service.ProcessPaymentAsync(It.IsAny<Domain.Payment>()))
                    .Throws(new Exception("System Error"));

                _paymentService = new PaymentService(_paymentRepository.Object, _bankingThirdPartyService.Object);
            }
            
            [Fact]
            public void ReturnsAnErrorResult()
            {
                var result = _paymentService.ProcessPaymentAsync(_inputPayment).Result;
                Assert.IsType<ErrorResult<BankPaymentResult>>(result);
                Assert.IsType<SystemError>(result.Error);
                Assert.Equal("System Error", result.Error.ErrorMessage);
                Assert.False(result.IsSuccessful);
            }
        }
    }
}